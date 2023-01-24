namespace WorkforceDataApi.Services;

public class AzureBlobStorageService : ICloudStorageService
{
    public Task ArchiveTpsExtractFile(string filename, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DownloadTpsExtractFile(string filename, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetPendingProcessingTpsExtractFilenames(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
