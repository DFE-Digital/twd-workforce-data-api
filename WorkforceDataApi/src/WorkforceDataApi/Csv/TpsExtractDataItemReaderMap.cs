using CsvHelper.Configuration;
using WorkforceDataApi.Models;

namespace WorkforceDataApi.Csv;

public class TpsExtractDataItemReaderMap : ClassMap<TpsExtractDataItem>
{
    public TpsExtractDataItemReaderMap()
    {
        Map(i => i.TpsExtractDataItemId).Index(0);
        Map(i => i.MemberPostcode).Index(1);
        Map(i => i.Trn).Index(2);
        Map(i => i.Nino).Index(3);
        Map(i => i.FirstName).Index(4);
        Map(i => i.LastName).Index(5);
        Map(i => i.DateOfBirth).Index(6).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EstablishmentPostcode).Index(7);
        Map(i => i.EmailAddress).Index(8);
        Map(i => i.LocalAuthorityNumber).Index(9);
        Map(i => i.EstablishmentNumber).Index(10);
        Map(i => i.EmploymentPeriodStartDate).Index(11).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EmploymentPeriodEndDate).Index(12).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.FullOrPartTimeIndicator).Index(13);
        Map(i => i.Created).Constant(DateTime.UtcNow);
        Map(i => i.Updated).Constant(DateTime.UtcNow);
        Map(i => i.TeachingStatus).Index(14);
    }
}
