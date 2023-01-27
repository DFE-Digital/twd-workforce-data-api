namespace WorkforceDataApi.DevUtils.Csv;

public interface IEstablishmentsCsvDownloader
{
    Task<string> DownloadLatest();
}
