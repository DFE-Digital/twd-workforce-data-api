using System.Globalization;
using Bogus;
using CsvHelper;
using WorkforceDataApi.DevUtils.Models;
using WorkforceDataApi.DevUtils.Csv;

namespace WorkforceDataApi.DevUtils.Services;

public class EstablishmentGenerationService : IEstablishmentGenerationService
{
    private readonly Randomizer establishmentRandomizer = new Randomizer();
    private Establishment[] _establishments;

    public EstablishmentGenerationService()
    {
        using var reader = new StreamReader("edubasealldata20230119.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EstablishmentReaderMap>();
        _establishments = csv.GetRecords<Establishment>()
            .Where(e => e.LocalAuthorityNumber != "000" && !string.IsNullOrEmpty(e.EstablishmentNumber) && !string.IsNullOrEmpty(e.EstablishmentPostcode)).ToArray();
    }

    public Establishment Generate()
    {
        var establishmentIndex = establishmentRandomizer.Number(0, _establishments.Length - 1);
        return _establishments[establishmentIndex];
    }    
}
