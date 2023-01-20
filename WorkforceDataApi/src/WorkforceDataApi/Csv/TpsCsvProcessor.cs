using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using WorkforceDataApi.Models;

namespace WorkforceDataApi.Csv;

public class TpsCsvProcessor : ITpsCsvProcessor
{
    private readonly WorkforceDbContext _dbContext;
    private readonly ILogger<TpsCsvProcessor> _logger;

    public TpsCsvProcessor(
        WorkforceDbContext dbContext,
        ILogger<TpsCsvProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Process(string csvFileName)
    {
        if (!File.Exists(csvFileName))
        {
            throw new FileNotFoundException($"The CSV file was not found at {csvFileName}");
        }

        using var reader = new StreamReader(csvFileName);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
        csv.Context.RegisterClassMap<TpsExtractDataItemReaderMap>();

        int i = 0;
        await foreach (var item in csv.GetRecordsAsync<TpsExtractDataItem>())
        {
            _dbContext.TpsExtractDataItems.Add(item);
            if (i != 0 && i % 10_000 == 0)
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Saved {i} TPS extract data items in Postgres workload database.");
            }

            i++;
        }

        if (_dbContext.ChangeTracker.HasChanges())
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Saved {i} TPS extract data items in Postgres workload database.");
        }
    }
}
