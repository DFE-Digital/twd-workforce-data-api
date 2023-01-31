using CsvHelper.Configuration;
using WorkforceDataApi.Models;

namespace WorkforceDataApi.DevUtils.Csv;

internal class TpsExtractDataItemWriterMap : ClassMap<TpsExtractDataItem>
{
    public TpsExtractDataItemWriterMap()
    {
        Map(i => i.TpsExtractDataItemId);
        Map(i => i.MemberId);
        Map(i => i.MemberPostcode);
        Map(i => i.Trn);
        Map(i => i.Nino);
        Map(i => i.FirstName);
        Map(i => i.LastName);
        Map(i => i.DateOfBirth).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EstablishmentPostcode);
        Map(i => i.EmailAddress);
        Map(i => i.LocalAuthorityNumber);
        Map(i => i.EstablishmentNumber);
        Map(i => i.EmploymentPeriodStartDate).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.EmploymentPeriodEndDate).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.FullOrPartTimeIndicator).Convert(i => ((int?)i.Value.FullOrPartTimeIndicator).ToString());
        Map(i => i.TeachingStatus);
    }
}
