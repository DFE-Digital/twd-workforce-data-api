using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace WorkforceDataApi.DevUtils.Csv;

public class EstablishmentsCsvDownloader : IEstablishmentsCsvDownloader
{
    private const string BaseDownloadUri = "https://ea-edubase-api-prod.azurewebsites.net/edubase/downloads/public/";
    private readonly HttpClient _httpClient;
    private readonly ILogger<EstablishmentsCsvDownloader> _logger;

    public EstablishmentsCsvDownloader(
        HttpClient httpClient,
        ILogger<EstablishmentsCsvDownloader> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> DownloadLatestToFile(string downloadFolder, bool overwriteExisting = false)
    {        
        _httpClient.BaseAddress = new Uri(BaseDownloadUri);

        var filename = GetLatestEstablishmentsCsvFilename();
        var fullFilename = Path.Combine(downloadFolder, filename);

        if (overwriteExisting || !File.Exists(fullFilename))
        {
            Directory.CreateDirectory(downloadFolder);

            _logger.LogInformation("Downloading latest Establishments CSV {filename}", filename);
            using var response = await _httpClient.GetAsync(filename);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            using var fs = new FileStream(fullFilename, FileMode.Create);
            await stream.CopyToAsync(fs);
            _logger.LogInformation("Latest Establishments CSV downloaded to {fullFilename}", fullFilename);
        }        

        return fullFilename;
    }

    private string GetLatestEstablishmentsCsvFilename()
    {
        var filename = $"edubasealldata{DateTime.Today:yyyyMMdd}.csv";
        return filename ;
    }
}
