using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace WorkforceDataApi.Services;

public class AzureTpsExtractRemoteStorageService : ITpsExtractRemoteStorageService
{
    private const string PendingFolderName = "pending";
    private const string ProcessedFolderName = "processed";
    private readonly string _connectionString;
    private readonly string _tpsExtractBlobContainerName;

    public AzureTpsExtractRemoteStorageService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BlobStorageConnection") ??
            throw new Exception("Connection string BlobStorageConnection is missing.");
        _tpsExtractBlobContainerName = configuration.GetValue("TpsExtractBlobContainerName", "tps-extract") ?? "tps-extract";
    }

    public async Task<string[]> GetPendingProcessingTpsExtractFilenames(CancellationToken cancellationToken)
    {
        var blobContainerClient = new BlobContainerClient(_connectionString, _tpsExtractBlobContainerName);
        var fileNames = new List<string>();
        await GetFileNamesAsync(blobContainerClient, PendingFolderName, true, fileNames, cancellationToken);
        return fileNames.ToArray();
    }

    public async Task<string> DownloadTpsExtractFile(string filename, string downloadFolder, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(downloadFolder);

        var filenameParts = filename.Split("/");
        var filenameWithoutFolder = filenameParts.Last();

        var blobContainerClient = new BlobContainerClient(_connectionString, _tpsExtractBlobContainerName);
        var blobClient = blobContainerClient.GetBlobClient(filename);
        var fileInfo = new FileInfo(Path.Combine(downloadFolder, filenameWithoutFolder));
        using var fs = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);
        await blobClient.DownloadToAsync(fs, cancellationToken);

        return fileInfo.FullName;
    }

    public async Task ArchiveTpsExtractFile(string filename, CancellationToken cancellationToken)
    {
        var blobContainerClient = new BlobContainerClient(_connectionString, _tpsExtractBlobContainerName);

        var sourceBlobClient = blobContainerClient.GetBlobClient(filename);
        if (await sourceBlobClient.ExistsAsync(cancellationToken))
        {
            var filenameParts = filename.Split("/");
            var filenameWithoutFolder = filenameParts.Last();
            var targetFilename = $"{ProcessedFolderName}/{filenameWithoutFolder}"; 

            // Acquire a lease to prevent another client modifying the source blob
            var lease = sourceBlobClient.GetBlobLeaseClient();
            await lease.AcquireAsync(TimeSpan.FromSeconds(60), cancellationToken: cancellationToken);

            var targetBlobClient = blobContainerClient.GetBlobClient(targetFilename);
            var copyOperation = await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, cancellationToken: cancellationToken);
            await copyOperation.WaitForCompletionAsync();

            // Release the lease
            var sourceProperties = await sourceBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (sourceProperties.Value.LeaseState == LeaseState.Leased)
            {
                await lease.ReleaseAsync(cancellationToken: cancellationToken);
            }

            // Now remove the original blob
            await sourceBlobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
    }

    private async Task GetFileNamesAsync(BlobContainerClient containerClient, string prefix, bool includeSubfolders, List<string> filenames, CancellationToken cancellationToken)
    {
        var resultSegment = containerClient.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/", cancellationToken: cancellationToken).AsPages();

        // Enumerate the blobs returned for each page.
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
            {
                // A hierarchical listing may return both virtual directories and blobs.
                if (blobhierarchyItem.IsPrefix)
                {
                    if (includeSubfolders)
                    {
                        // Call recursively with the prefix to traverse the virtual directory.
                        await GetFileNamesAsync(containerClient, blobhierarchyItem.Prefix, true, filenames, cancellationToken);
                    }
                }
                else
                {
                    filenames.Add(blobhierarchyItem.Blob.Name);
                }
            }
        }
    }
}
