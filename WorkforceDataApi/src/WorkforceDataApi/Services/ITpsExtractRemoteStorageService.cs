namespace WorkforceDataApi.Services;

public interface ITpsExtractRemoteStorageService
{
    Task<string[]> GetPendingProcessingTpsExtractFilenames(CancellationToken cancellationToken);

    Task<string> DownloadTpsExtractFile(string filename, string downloadFolder, CancellationToken cancellationToken);

    Task ArchiveTpsExtractFile(string filename, CancellationToken cancellationToken);
}
