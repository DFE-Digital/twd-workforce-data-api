namespace WorkforceDataApi.DevUtils.Csv;

public interface IEstablishmentsCsvDownloader
{
    Task<string> DownloadLatestToFile(string downloadFolder, bool overwriteExisting = false);
}
