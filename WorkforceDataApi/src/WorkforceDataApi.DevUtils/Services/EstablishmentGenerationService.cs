using System.Globalization;
using Bogus;
using CsvHelper;
using WorkforceDataApi.DevUtils.Models;
using WorkforceDataApi.DevUtils.Csv;

namespace WorkforceDataApi.DevUtils.Services;

public class EstablishmentGenerationService : IEstablishmentGenerationService
{
    private readonly Randomizer establishmentRandomizer = new Randomizer();
    private EstablishmentSummary[] _establishments;

    public EstablishmentGenerationService()
    {
        using var reader = new StreamReader("edubasealldata20230119.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EstablishmentSummaryReaderMap>();
        _establishments = csv.GetRecords<EstablishmentSummary>()
            .Where(e => e.LocalAuthorityNumber != "000" && !string.IsNullOrEmpty(e.EstablishmentNumber) && !string.IsNullOrEmpty(e.EstablishmentPostcode)).ToArray();
    }

    public EstablishmentSummary Generate()
    {
        var establishmentIndex = establishmentRandomizer.Number(0, _establishments.Length - 1);
        return _establishments[establishmentIndex];
    }    
}
