using CsvHelper.Configuration;
using WorkforceDataApi.DevUtils.Models;

namespace WorkforceDataApi.DevUtils.Csv;

public class EstablishmentSummaryReaderMap : ClassMap<EstablishmentSummary>
{
    public EstablishmentSummaryReaderMap()
    {
        Map(i => i.LocalAuthorityNumber).Index(1);
        Map(i => i.EstablishmentNumber).Index(3);
        Map(i => i.EstablishmentName).Index(4);
        Map(i => i.EstablishmentPostcode).Index(64);
    }
}
