using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using WorkforceDataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkforceDataApi.DevUtils.Csv;

public class EstablishmentsCsvProcessor : IEstablishmentsCsvProcessor
{
    private readonly WorkforceDbContext _dbContext;
    private readonly ILogger<EstablishmentsCsvProcessor> _logger;

    public EstablishmentsCsvProcessor(
        WorkforceDbContext dbContext,
        ILogger<EstablishmentsCsvProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Process(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"The CSV file was not found at {filename}");
        }

        // Wipe establishments as we will be refreshing all of the data
        await _dbContext.Database.ExecuteSqlRawAsync("Truncate table establishments_raw");

        using var reader = new StreamReader(filename);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
        csv.Context.RegisterClassMap<EstablishmentRawReaderMap>();

        int i = 0;
        await foreach (var item in csv.GetRecordsAsync<EstablishmentRaw>())
        {
            _dbContext.Establishments.Add(item);
            if (i != 0 && i % 2_000 == 0)
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Saved {i} establishments in Postgres workload database.", i);
            }

            i++;
        }

        if (_dbContext.ChangeTracker.HasChanges())
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Saved {i} establishments in Postgres workload database.", i);
        }
    }
}
