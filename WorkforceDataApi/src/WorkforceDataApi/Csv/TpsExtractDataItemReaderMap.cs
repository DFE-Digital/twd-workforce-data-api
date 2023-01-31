using CsvHelper.Configuration;
using WorkforceDataApi.Models;

namespace WorkforceDataApi.Csv;

public class TpsExtractDataItemReaderMap : ClassMap<TpsExtractDataItem>
{
    public TpsExtractDataItemReaderMap()
    {
        Map(i => i.TpsExtractDataItemId).Index(0);
        Map(i => i.MemberId).Index(1);
        Map(i => i.MemberPostcode).Index(2);
        Map(i => i.Trn).Index(3);
        Map(i => i.Nino).Index(4);
        Map(i => i.FirstName).Index(5);
        Map(i => i.LastName).Index(6);
        Map(i => i.DateOfBirth).Index(7).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EstablishmentPostcode).Index(8).TypeConverterOption.NullValues("");
        Map(i => i.EmailAddress).Index(9);
        Map(i => i.LocalAuthorityNumber).Index(10).TypeConverterOption.NullValues("");
        Map(i => i.EstablishmentNumber).Index(11).TypeConverterOption.NullValues("");
        Map(i => i.EmploymentPeriodStartDate).Index(12).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EmploymentPeriodEndDate).Index(13).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.FullOrPartTimeIndicator).Index(14);
        Map(i => i.Created).Constant(DateTime.UtcNow);
        Map(i => i.Updated).Constant(DateTime.UtcNow);
        Map(i => i.TeachingStatus).Index(15);
    }
}
