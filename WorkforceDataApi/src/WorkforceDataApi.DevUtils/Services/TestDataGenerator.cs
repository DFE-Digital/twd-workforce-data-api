using Bogus;
using Bogus.Extensions.UnitedKingdom;
using Microsoft.Extensions.Logging;
using TeacherIdentity.AuthServer.Models;
using WorkforceDataApi.DevUtils.Models;
using WorkforceDataApi.Models;
using static Bogus.DataSets.Name;

namespace WorkforceDataApi.DevUtils.Services;

public class TestDataGenerator
{
    private readonly Randomizer commonRandomizer = new Randomizer();
    private readonly IEstablishmentGenerationService _establishmentGenerationService;
    private readonly ILogger<TestDataGenerator> _logger;

    public TestDataGenerator(
        IEstablishmentGenerationService establishmentGenerationService,
        ILogger<TestDataGenerator> logger)
    {
        _establishmentGenerationService = establishmentGenerationService;
        _logger = logger;
    }

    /// <summary>
    /// Generate Test Data with as realistic distribution of data representing teacher workforce data over a 12 month period.
    /// </summary>
    /// <remarks>
    /// The test data will be generated based on the following default distribution:
    /// 12 months total span
    /// 700,000 teachers
    /// 9% change jobs once during the 12 months
    /// 1% are supply teachers working in up to 3 schools per month - 50% work in 2 schools over 12 months and 50% work in 4 different schools over 12 months
    /// 10% are part-time teachers (80% regular / 20% irregular)
    /// 10% of teachers start teaching in a given year
    /// 8% of teachers leave teaching in a given year
    /// </remarks>
    /// <returns>
    /// An enumerable list if TPS extract data items.
    /// </returns>
    public IEnumerable<WorkforceData> GenerateTestData(
        DateOnly startDate,
        int months = 12,
        int teacherCount = 700_000,
        int changeJobOncePercentage = 9,
        int supplyTeacherPercentage = 1,
        int newStarterPercentage = 10,
        int leaverPercentage = 8,
        int partTimePercentage = 10)
    {
        int partTimeRegularPercentage = (int)Math.Round(partTimePercentage * 0.8);
        int partTimeIrregularPercentage = (int)Math.Round(partTimePercentage * 0.2);

        int teachersGenerated = 0;
        int leaversGenerated = 0;
        int newStartersGenerated = 0;
        int supplyTeachersGenerated = 0;
        int changeJobTeachersGenerated = 0;
        int noChangeTeachersGenerated = 0;
        int partTimeRegularGenerated = 0;
        int partTimeIrregularGenerated = 0;

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

        // Generate all possible TRNs and randomise list to use to generate unique TRNs
        var trns = Enumerable.Range(1000000, 8999999);
        var randomised = commonRandomizer.Shuffle(trns);
        var enumerator = randomised.GetEnumerator();

        var teacherFaker = GetTeacherFaker(() =>
        {
            if (!enumerator.MoveNext())
            {
                enumerator.Reset();
                enumerator.MoveNext();
            }

            var trn = enumerator.Current;
            return trn.ToString();
        },
        () =>
        {
            // Default Distribution we'll use is:
            // 90% FullTimeEmployment
            // 8% PartTimeRegular
            // 2% IrregularPartTime
            var weight = commonRandomizer.Number(1, 100);
            FullOrPartTimeIndicatorType indicator;
            if (weight <= partTimeIrregularPercentage)
            {
                indicator = FullOrPartTimeIndicatorType.IrregularPartTime;
            }
            else if (weight <= (partTimeIrregularPercentage + partTimeRegularPercentage))
            {
                indicator = FullOrPartTimeIndicatorType.PartTimeRegular;
            }
            else
            {
                indicator = FullOrPartTimeIndicatorType.FullTimeEmployment;
            }

            return indicator;
        });

        var workforceDataFaker = new Faker<WorkforceData>("en_GB")
            .CustomInstantiator((f) =>
            {
                var teacher = teacherFaker.Generate();
                teachersGenerated++;
                if (teacher.FullOrPartTimeIndicator == FullOrPartTimeIndicatorType.PartTimeRegular)
                {
                    partTimeRegularGenerated++;
                }
                else if (teacher.FullOrPartTimeIndicator == FullOrPartTimeIndicatorType.IrregularPartTime)
                {
                    partTimeIrregularGenerated++;
                }

                DateOnly? newStarterStartDate = null;
                DateOnly? leaverEndDate = null;

                // Now generate their employment history based on expected distributions of data.
                var tpsExtractDataItems = new List<TpsExtractDataItem>();
                
                // Default Data distribution weight for existing, new or leaving teacher is
                // existing: 80%
                // new: 10%
                // leaver: 8% 
                var weight = commonRandomizer.Number(1, 100);
                if (weight <= leaverPercentage)
                {
                    // Leaver
                    leaverEndDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    leaversGenerated++;
                }
                else if (weight > leaverPercentage && weight <= (leaverPercentage + newStarterPercentage))
                {
                    // New
                    newStarterStartDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    newStartersGenerated++;
                }
                else
                {
                    // Existing
                }

                // Default Data distribution weight for stay at same school, change schools, supply teacher is
                // same: 90%
                // change: 9%
                // supply: 1%
                // this is skewed slightly with the assumption that new starters would not tend to change schools within 12 months 
                weight = commonRandomizer.Number(1, 100);
                if (weight <= supplyTeacherPercentage)
                {
                    var numberOfDifferentSchools = f.PickRandom(2, 4);
                    var schools = new List<Establishment>();
                    for (int i = 0; i < numberOfDifferentSchools; i++)
                    {
                        schools.Add(_establishmentGenerationService.Generate());
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
                            MemberId = teacher.MemberId,
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
                        MemberId = teacher.MemberId,
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
                            MemberId = teacher.MemberId,
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
                else if ((weight > supplyTeacherPercentage && weight <= (supplyTeacherPercentage + changeJobOncePercentage)) && !newStarterStartDate.HasValue)
                {
                    // Changed schools once during year (unlikely for new starters I would have thought??)
                    var changeSchoolStartDate = f.Date.BetweenDateOnly(startDateRange.AddDays(1), endDateRange.AddDays(-1));
                    var created = f.Date.Recent();

                    var firstSchool = _establishmentGenerationService.Generate();
                    var firstSchoolDataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        MemberId = teacher.MemberId,
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

                    var secondSchool = _establishmentGenerationService.Generate();
                    var secondSchoolDataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        MemberId = teacher.MemberId,
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
                            MemberId = teacher.MemberId,
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
                    var school = _establishmentGenerationService.Generate();
                    var dataItem = new TpsExtractDataItem
                    {
                        TpsExtractDataItemId = Guid.NewGuid().ToString(),
                        MemberId = teacher.MemberId,
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
                            MemberId = teacher.MemberId,
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

                var workforceData = new WorkforceData
                {
                    Teacher = teacher,
                    WorkforceDataItems = tpsExtractDataItems.ToArray()
                };
                
                return workforceData;
            });

        foreach (var item in workforceDataFaker.GenerateLazy(teacherCount))
        {
            yield return item;
        }

        var leaversActualPercentage = Math.Round((double) (leaversGenerated * 100) / teachersGenerated, 2);
        var newStartersActualPercentage = Math.Round((double)(newStartersGenerated * 100) / teachersGenerated, 2);
        var supplyTeachersActualPercentage = Math.Round((double)(supplyTeachersGenerated * 100) / teachersGenerated, 2);
        var changeJobTeachersActualPercentage = Math.Round((double)(changeJobTeachersGenerated * 100) / teachersGenerated, 2);
        var noChangeTeachersActualPercentage = Math.Round((double)(noChangeTeachersGenerated * 100) / teachersGenerated, 2);
        var partTimeRegularActualPercentage = Math.Round((double)(partTimeRegularGenerated * 100) / teachersGenerated, 2);
        var partTimeIrregularActualPercentage = Math.Round((double)(partTimeIrregularGenerated * 100) / teachersGenerated, 2);

        _logger.LogInformation("Total teachers generated                     = {teachersGenerated}", teachersGenerated);
        _logger.LogInformation("Total part-time regular teachers generated   = {leaversGenerated} ({partTimeRegularActualPercentage}%)", partTimeRegularGenerated, partTimeRegularActualPercentage);
        _logger.LogInformation("Total part-time irregular teachers generated = {leaversGenerated} ({partTimeIrregularActualPercentage}%)", partTimeIrregularGenerated, partTimeIrregularActualPercentage);
        _logger.LogInformation("Total new starters generated                 = {newStartersGenerated} ({newStartersActualPercentage}%)", newStartersGenerated, newStartersActualPercentage);
        _logger.LogInformation("Total leavers generated                      = {leaversGenerated} ({leaversActualPercentage}%)", leaversGenerated, leaversActualPercentage);
        _logger.LogInformation("Total supply teachers generated              = {supplyTeachersGenerated} ({supplyTeachersActualPercentage}%)", supplyTeachersGenerated, supplyTeachersActualPercentage);
        _logger.LogInformation("Total teachers changing schools once         = {changeJobTeachersGenerated} ({changeJobTeachersActualPercentage}%)", changeJobTeachersGenerated, changeJobTeachersActualPercentage);
        _logger.LogInformation("Total teachers staying at same school        = {noChangeTeachersGenerated} ({noChangeTeachersActualPercentage}%)", noChangeTeachersGenerated, noChangeTeachersActualPercentage);
    }

    private Faker<Teacher> GetTeacherFaker(Func<string> trnGenerator, Func<FullOrPartTimeIndicatorType> fullOrPartTimeGenerator)
    {
        var teacherFaker = new Faker<Teacher>("en_GB")
           .RuleFor(i => i.MemberId, (f, i) => Guid.NewGuid().ToString())
           .RuleFor(i => i.TeachingStatus, (f, i) => commonRandomizer.TeachingStatus())
           .RuleFor(i => i.Trn, (f, i) => trnGenerator())
           .RuleFor(i => i.FirstName, (f, i) => f.Name.FirstName(f.PickRandom<Gender>()))
           .RuleFor(i => i.LastName, (f, i) => f.Name.LastName())
           .RuleFor(i => i.Nino, (f, i) => f.Finance.Nino().Replace(" ", string.Empty))
           .RuleFor(i => i.DateOfBirth, (f, i) => f.Date.BetweenDateOnly(new DateOnly(1950, 1, 1), new DateOnly(2000, 1, 1)))
           .RuleFor(i => i.EmailAddress, (f, i) => f.Internet.Email(i.FirstName, i.LastName, uniqueSuffix: commonRandomizer.Number(1, 1000000).ToString()))
           .RuleFor(i => i.MemberPostcode, (f, i) => f.Address.ZipCode())
           .RuleFor(i => i.FullOrPartTimeIndicator, (f, i) => fullOrPartTimeGenerator());

        return teacherFaker;
    }
}

public class DateRange
{
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
