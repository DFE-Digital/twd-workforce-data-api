namespace WorkforceDataApi.DevUtils.Csv;

public interface IEstablishmentsCsvDownloader
{
    string GetLatestEstablishmentsCsvFilename();

    Task<string> DownloadLatest();
}
