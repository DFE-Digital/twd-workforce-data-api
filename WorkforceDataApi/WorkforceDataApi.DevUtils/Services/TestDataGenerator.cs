using Bogus;
using Bogus.Extensions.UnitedKingdom;
using TeacherIdentity.AuthServer.Models;
using WorkforceDataApi.Models;
using static Bogus.DataSets.Name;

namespace WorkforceDataApi.DevUtils.Services;

public class TestDataGenerator
{
    private readonly Randomizer commonRandomizer = new Randomizer();

    private int teachersGenerated = 0;
    private int leaversGenerated = 0;
    private int newStartersGenerated = 0;
    private int supplyTeachersGenerated = 0;
    private int changeJobTeachersGenerated = 0;
    private int noChangeTeachersGenerated = 0;

    /// <summary>
    /// Generate Test Data with as realistic distribution of data representing teacher workforce data over a 12 month period.
    /// </summary>
    /// <remarks>
    /// The test data will be generated based on the following distribution:
    /// 12 months total span
    /// 700,000 teachers
    /// 9% change jobs during the 12 months
    /// 1% are supply teachers working in up to 3 schools per month - 50% work in 2 schools over 12 months and 50% work in 4 different schools over 12 months
    /// 8% are regular part-time teachers
    /// 2% are irregular part-time teachers
    /// 10% of teachers start teaching in a given year
    /// 8% of teachers leave teaching in a given year
    /// </remarks>
    /// <returns>
    /// An enumerable list if TPS extract data items.
    /// </returns>
    public IEnumerable<TpsExtractDataItem> GenerateTestData(DateOnly startDate, int months = 12, int teacherCount = 700_000)
    {
        List<DateRange> dateRanges = new List<DateRange>();
        var startDateRange = new DateOnly(startDate.Year, startDate.Month, 1);
        var startOfMonth = startDateRange;
        DateOnly endOfMonth = new DateOnly(startOfMonth.Year, startOfMonth.Month, DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month));
        for (int i = 0; i < months; i++)
        {
            endOfMonth = new DateOnly(startOfMonth.Year, startOfMonth.Month, DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month));
            dateRanges.Add(new DateRange { StartDate = startOfMonth, EndDate = endOfMonth });

            startOfMonth = endOfMonth.AddDays(1);
        }

        var endDateRange = endOfMonth;

        var teacherFaker = GetTeacherFaker();
        var establishmentFaker = GetEstablishmentFaker();
        var faker = new Faker<TpsExtractDataItem[]>("en_GB")
            .CustomInstantiator((f) =>
            {
                var teacher = teacherFaker.Generate();
                teachersGenerated++;
                DateOnly? newStarterStartDate = null;
                DateOnly? leaverEndDate = null;

                // Now generate their employment history based on expected distributions of data.
                var tpsExtractDataItems = new List<TpsExtractDataItem>();
                
                // Data distribution weight for existing, new or leaving teacher is
                // existing: 80%
                // new: 10%
                // leaver: 8% 
                var weight = commonRandomizer.Number(1, 100);
                if (weight < 9)
                {
                    // Leaver
                    leaverEndDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    leaversGenerated++;
                }
                else if (weight > 8 && weight < 19)
                {
                    // New
                    newStarterStartDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    newStartersGenerated++;
                }
                else
                {
                    // Existing
                }

                // Data distribution weight for stay at same school, change schools, supply teacher is
                // same: 90%
                // change: 9%
                // supply: 1%
                // this is skewed slightly with the assumption that new starters would not tend to change schools within 12 months 
                weight = commonRandomizer.Number(1, 100);
                if (weight < 2)
                {
                    var numberOfDifferentSchools = f.PickRandom(2, 4);
                    var schools = new List<Establishment>();
                    for (int i = 0; i < numberOfDifferentSchools; i++)
                    {
                        schools.Add(establishmentFaker.Generate());
                    }

                    var availableSchools = schools.ToArray();                                        
                    var created = f.Date.Recent();                    

                    // Supply teacher working at 1 - 3 schools per month i.e. between 0 and 2 school changes in a month
                    // Let's say that if it's 1 change it can happen any time in the month, if it's 2 then it will happen any time up to 15th + any time from 16th to the end of the month
                    // If they are a new starter then don't have any changes the first month
                    // If they are a leaver then don't have any changes in the last month other than the status update
                    // 50% work at 2 different schools over the 12 months and 50% work at 4 different schools
                    var changeDates = new List<DateOnly>();
                    foreach (var dateRange in dateRanges)
                    {
                        // If new starter then skip this month if not starting in it
                        if (newStarterStartDate != null && newStarterStartDate > dateRange.EndDate)
                        {
                            continue;
                        }

                        // if leaver then exit if leaving this month
                        if (leaverEndDate != null && leaverEndDate >= dateRange.StartDate && leaverEndDate <= dateRange.EndDate)
                        {
                            break;
                        }

                        var numberOfSchoolChanges = commonRandomizer.Number(0, 2);
                        if (numberOfSchoolChanges == 1)
                        {
                            // Change any day in the month
                            var changeDay = commonRandomizer.Number(1, dateRange.EndDate.Day);
                            changeDates.Add(new DateOnly(dateRange.StartDate.Year, dateRange.StartDate.Month, changeDay));

                        }
                        else if (numberOfSchoolChanges == 2)
                        {
                            // 1st change any day from 1st - 15th of month
                            var changeDay1 = commonRandomizer.Number(1, dateRange.EndDate.Day);
                            changeDates.Add(new DateOnly(dateRange.StartDate.Year, dateRange.StartDate.Month, changeDay1));
                            // 2nd change any day from 16th - end of month
                            var changeDay2 = commonRandomizer.Number(16, dateRange.EndDate.Day);
                            changeDates.Add(new DateOnly(dateRange.StartDate.Year, dateRange.StartDate.Month, changeDay2));
                        }               
                    }

                    DateOnly employmentPeriodStartDate = newStarterStartDate ?? startDateRange;
                    int currentSchoolIndex = 0;

                    // Now we have a list of dates when the teacher changes school, build up the data to represent that
                    foreach (var changeDate in changeDates)
                    {
                        var currentSchool = availableSchools[currentSchoolIndex];
                        var dataItem = new TpsExtractDataItem
                        {
                            TpsExtractDataItemId = Guid.NewGuid().ToString(),
                            TeachingStatus = teacher.TeachingStatus,
                            Trn = teacher.Trn,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Nino = teacher.Nino,
                            DateOfBirth = teacher.DateOfBirth,
                            EmailAddress = teacher.EmailAddress,
                            MemberPostcode = teacher.MemberPostcode,
                            FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                            LocalAuthorityNumber = currentSchool.LocalAuthorityNumber,
                            EstablishmentNumber = currentSchool.EstablishmentNumber,
                            EstablishmentPostcode = currentSchool.EstablishmentPostcode,
                            EmploymentPeriodStartDate = employmentPeriodStartDate,
                            EmploymentPeriodEndDate = changeDate.AddDays(-1),
                            Created = created,
                            Updated = created
                        };

                        tpsExtractDataItems.Add(dataItem);

                        currentSchoolIndex = (currentSchoolIndex == numberOfDifferentSchools - 1) ? 0 : currentSchoolIndex + 1;
                        employmentPeriodStartDate = changeDate;
                    }

                    // Write out final record to the end the year or leaver date
                    var lastSchool = availableSchools[currentSchoolIndex];
                    var lastSchoolDataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        TeachingStatus = teacher.TeachingStatus,
                        Trn = teacher.Trn,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        Nino = teacher.Nino,
                        DateOfBirth = teacher.DateOfBirth,
                        EmailAddress = teacher.EmailAddress,
                        MemberPostcode = teacher.MemberPostcode,
                        FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                        LocalAuthorityNumber = lastSchool.LocalAuthorityNumber,
                        EstablishmentNumber = lastSchool.EstablishmentNumber,
                        EstablishmentPostcode = lastSchool.EstablishmentPostcode,
                        EmploymentPeriodStartDate = employmentPeriodStartDate,
                        EmploymentPeriodEndDate = leaverEndDate ?? endDateRange,
                        Created = created,
                        Updated = created
                    };

                    tpsExtractDataItems.Add(lastSchoolDataItem);

                    // If leaver then for the moment we are going to add an "L" status record starting the day after leaving
                    if (leaverEndDate.HasValue)
                    {
                        var leaverDataItem = new TpsExtractDataItem
                        {
                            TpsExtractDataItemId = Guid.NewGuid().ToString(),
                            TeachingStatus = "L",
                            Trn = teacher.Trn,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Nino = teacher.Nino,
                            DateOfBirth = teacher.DateOfBirth,
                            EmailAddress = teacher.EmailAddress,
                            MemberPostcode = teacher.MemberPostcode,
                            FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                            LocalAuthorityNumber = lastSchool.LocalAuthorityNumber,
                            EstablishmentNumber = lastSchool.EstablishmentNumber,
                            EstablishmentPostcode = lastSchool.EstablishmentPostcode,
                            EmploymentPeriodStartDate = leaverEndDate.Value.AddDays(1),
                            EmploymentPeriodEndDate = endDateRange,
                            Created = created,
                            Updated = created
                        };

                        tpsExtractDataItems.Add(leaverDataItem);
                    }

                    supplyTeachersGenerated++;
                }
                else if ((weight > 1 && weight < 11) && !newStarterStartDate.HasValue)
                {
                    // Changed schools once during year (unlikely for new starters I would have thought??)
                    var changeSchoolStartDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    var created = f.Date.Recent();

                    var firstSchool = establishmentFaker.Generate();
                    var firstSchoolDataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        TeachingStatus = teacher.TeachingStatus,
                        Trn = teacher.Trn,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        Nino = teacher.Nino,
                        DateOfBirth = teacher.DateOfBirth,
                        EmailAddress = teacher.EmailAddress,
                        MemberPostcode = teacher.MemberPostcode,
                        FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                        LocalAuthorityNumber = firstSchool.LocalAuthorityNumber,
                        EstablishmentNumber = firstSchool.EstablishmentNumber,
                        EstablishmentPostcode = firstSchool.EstablishmentPostcode,
                        EmploymentPeriodStartDate = startDateRange,
                        EmploymentPeriodEndDate = changeSchoolStartDate.AddDays(-1),
                        Created = created,
                        Updated = created
                    };

                    var secondSchool = establishmentFaker.Generate();
                    var secondSchoolDataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        TeachingStatus = teacher.TeachingStatus,
                        Trn = teacher.Trn,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        Nino = teacher.Nino,
                        DateOfBirth = teacher.DateOfBirth,
                        EmailAddress = teacher.EmailAddress,
                        MemberPostcode = teacher.MemberPostcode,
                        FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                        LocalAuthorityNumber = secondSchool.LocalAuthorityNumber,
                        EstablishmentNumber = secondSchool.EstablishmentNumber,
                        EstablishmentPostcode = secondSchool.EstablishmentPostcode,
                        EmploymentPeriodStartDate = changeSchoolStartDate,
                        EmploymentPeriodEndDate = leaverEndDate ?? endDateRange,
                        Created = created,
                        Updated = created
                    };                    

                    tpsExtractDataItems.Add(firstSchoolDataItem);
                    tpsExtractDataItems.Add(secondSchoolDataItem);

                    // If leaver then for the moment we are going to add an "L" status record starting the day after leaving
                    if (leaverEndDate.HasValue)
                    {
                        var leaverDataItem = new TpsExtractDataItem
                        {
                            TpsExtractDataItemId = Guid.NewGuid().ToString(),
                            TeachingStatus = "L",
                            Trn = teacher.Trn,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Nino = teacher.Nino,
                            DateOfBirth = teacher.DateOfBirth,
                            EmailAddress = teacher.EmailAddress,
                            MemberPostcode = teacher.MemberPostcode,
                            FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                            LocalAuthorityNumber = secondSchool.LocalAuthorityNumber,
                            EstablishmentNumber = secondSchool.EstablishmentNumber,
                            EstablishmentPostcode = secondSchool.EstablishmentPostcode,
                            EmploymentPeriodStartDate = leaverEndDate.Value.AddDays(1),
                            EmploymentPeriodEndDate = endDateRange,
                            Created = created,
                            Updated = created
                        };

                        tpsExtractDataItems.Add(leaverDataItem);
                    }

                    changeJobTeachersGenerated++;
                }
                else
                {
                    // Same school all year
                    var created = f.Date.Recent();
                    var school = establishmentFaker.Generate();
                    var dataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        TeachingStatus = teacher.TeachingStatus,
                        Trn = teacher.Trn,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        Nino = teacher.Nino,
                        DateOfBirth = teacher.DateOfBirth,
                        EmailAddress = teacher.EmailAddress,
                        MemberPostcode = teacher.MemberPostcode,
                        FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                        LocalAuthorityNumber = school.LocalAuthorityNumber,
                        EstablishmentNumber = school.EstablishmentNumber,
                        EstablishmentPostcode = school.EstablishmentPostcode,
                        EmploymentPeriodStartDate = newStarterStartDate ?? startDateRange,
                        EmploymentPeriodEndDate = leaverEndDate ?? endDateRange,
                        Created = created,
                        Updated = created
                    };

                    tpsExtractDataItems.Add(dataItem);

                    // If leaver then for the moment we are going to add an "L" status record starting the day after leaving
                    if (leaverEndDate.HasValue)
                    {
                        var leaverDataItem = new TpsExtractDataItem
                        {
                            TpsExtractDataItemId = Guid.NewGuid().ToString(),
                            TeachingStatus = "L",
                            Trn = teacher.Trn,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Nino = teacher.Nino,
                            DateOfBirth = teacher.DateOfBirth,
                            EmailAddress = teacher.EmailAddress,
                            MemberPostcode = teacher.MemberPostcode,
                            FullOrPartTimeIndicator = teacher.FullOrPartTimeIndicator,
                            LocalAuthorityNumber = school.LocalAuthorityNumber,
                            EstablishmentNumber = school.EstablishmentNumber,
                            EstablishmentPostcode = school.EstablishmentPostcode,
                            EmploymentPeriodStartDate = leaverEndDate.Value.AddDays(1),
                            EmploymentPeriodEndDate = endDateRange,
                            Created = created,
                            Updated = created
                        };

                        tpsExtractDataItems.Add(leaverDataItem);
                    }

                    noChangeTeachersGenerated++;
                }
                
                return tpsExtractDataItems.ToArray();
            });


        foreach (var item in faker.GenerateLazy(teacherCount).SelectMany(i => i))
        {
            yield return item;
        }

        Console.WriteLine($"Total teachers generated              = {teachersGenerated}");
        Console.WriteLine($"Total leavers generated               = {leaversGenerated}");
        Console.WriteLine($"Total new starters generated          = {newStartersGenerated}");
        Console.WriteLine($"Total supply teachers generated       = {supplyTeachersGenerated}");
        Console.WriteLine($"Total teachers changing schools once  = {changeJobTeachersGenerated}");
        Console.WriteLine($"Total teachers staying at same school = {noChangeTeachersGenerated}");
    }

    private Faker<Teacher> GetTeacherFaker()
    {
        var teacherFaker = new Faker<Teacher>("en_GB")
           .RuleFor(i => i.TeachingStatus, (f, i) => commonRandomizer.TeachingStatus())
           .RuleFor(i => i.Trn, (f, i) => commonRandomizer.Number(1000000, 9999999).ToString())
           .RuleFor(i => i.FirstName, (f, i) => f.Name.FirstName(f.PickRandom<Gender>()))
           .RuleFor(i => i.LastName, (f, i) => f.Name.LastName())
           .RuleFor(i => i.Nino, (f, i) => f.Finance.Nino().Replace(" ", string.Empty))
           .RuleFor(i => i.DateOfBirth, (f, i) => f.Date.BetweenDateOnly(new DateOnly(1950, 1, 1), new DateOnly(2000, 1, 1)))
           .RuleFor(i => i.EmailAddress, (f, i) => f.Internet.Email(i.FirstName, i.LastName))
           .RuleFor(i => i.MemberPostcode, (f, i) => f.Address.ZipCode())
           .RuleFor(i => i.FullOrPartTimeIndicator, (f, i) => commonRandomizer.FullOrPartTimeIndicator());

        return teacherFaker;
    }

    private Faker<Establishment> GetEstablishmentFaker()
    {
        var establishmentFaker = new Faker<Establishment>("en_GB")
           .RuleFor(i => i.LocalAuthorityNumber, (f, i) => commonRandomizer.LocalAuthorityNumber())
           .RuleFor(i => i.EstablishmentNumber, (f, i) => commonRandomizer.EstablishmentNumber())
           .RuleFor(i => i.EstablishmentPostcode, (f, i) => f.Address.ZipCode());

        return establishmentFaker;
    }
}

public class Teacher
{
    public required string TeachingStatus { get; init; }
    public required string Trn { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Nino { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string EmailAddress { get; init; }
    public required string MemberPostcode { get; init; }
    public required FullOrPartTimeIndicatorType FullOrPartTimeIndicator { get; init; }    
}

public class EmploymentDetail
{
    public required Establishment Establishment { get; init; } 
    public required DateOnly EmploymentPeriodStartDate { get; init; }
    public required DateOnly EmploymentPeriodEndDate { get; init; }
}

public class Establishment
{
    public required string LocalAuthorityNumber { get; init; }
    public required string EstablishmentNumber { get; init; }
    public required string EstablishmentPostcode { get; init; }
}

public class DateRange
{
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
