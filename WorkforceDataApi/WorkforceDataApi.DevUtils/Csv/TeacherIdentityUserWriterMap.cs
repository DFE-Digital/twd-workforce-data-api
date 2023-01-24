using CsvHelper.Configuration;
using WorkforceDataApi.DevUtils.Models.Identity;

namespace WorkforceDataApi.DevUtils.Csv;

internal class TeacherIdentityUserWriterMap : ClassMap<User>
{
    public TeacherIdentityUserWriterMap()
    {
        Map(i => i.UserId);
        Map(i => i.EmailAddress);
        Map(i => i.FirstName);
        Map(i => i.LastName);
        Map(i => i.DateOfBirth).TypeConverterOption.Format("ddMMyyyy");
        Map(i => i.UserType).Convert(i => ((int)(i.Value.UserType)).ToString());
        Map(i => i.Trn);
    }
}
