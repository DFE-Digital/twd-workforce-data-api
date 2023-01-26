namespace WorkforceDataApi.Services;

public interface ICloudStorageService
{
    Task<string[]> GetPendingProcessingTpsExtractFilenames();

    Task DownloadTpsExtractFile(string filename);

    Task ArchiveTpsExtractFile(string filename);
}
