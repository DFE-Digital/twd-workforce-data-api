namespace WorkforceDataApi.Services;

public interface ICloudStorageService
{
    Task<string[]> GetPendingProcessingTpsExtractFilenames(CancellationToken cancellationToken);

    Task DownloadTpsExtractFile(string filename, CancellationToken cancellationToken);

    Task ArchiveTpsExtractFile(string filename, CancellationToken cancellationToken);
}
