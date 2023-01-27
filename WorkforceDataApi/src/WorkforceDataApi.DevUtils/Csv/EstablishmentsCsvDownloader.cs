using System;
using System.Net.Http;

namespace WorkforceDataApi.DevUtils.Csv;

public class EstablishmentsCsvDownloader : IEstablishmentsCsvDownloader
{
    private const string BaseDownloadUri = "https://ea-edubase-api-prod.azurewebsites.net/edubase/downloads/public/";
    private readonly IHttpClientFactory _httpClientFactory;

    public EstablishmentsCsvDownloader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> DownloadLatest()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(BaseDownloadUri);

        var filename = $"edubasealldata{DateTime.Today.ToString("yyyyMMdd")}.csv";
        using var response = await httpClient.GetAsync(filename);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var fs = new FileStream(filename, FileMode.CreateNew);
        await stream.CopyToAsync(fs);

        return filename;
    }
}
