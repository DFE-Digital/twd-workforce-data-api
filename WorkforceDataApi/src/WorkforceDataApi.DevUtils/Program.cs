using System.CommandLine;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WorkforceDataApi.Csv;
using WorkforceDataApi.DevUtils.Csv;
using WorkforceDataApi.DevUtils.Models;
using WorkforceDataApi.DevUtils.Models.Identity;
using WorkforceDataApi.DevUtils.Services;
using WorkforceDataApi.Models;

var rootCommand = CreateRootCommand();
return await rootCommand.InvokeAsync(args);

static RootCommand CreateRootCommand()
{
    var migrateDatabaseCommand = CreateMigrateDatabaseCommand();
    var generateMockDataCommand = CreateGenerateMockDataCommand();
    var importTpsCsvCommand = CreateImportTpsCsvCommand();

    var rootCommand = new RootCommand("Workforce Data command line developer utilities.")
    {
        migrateDatabaseCommand,
        generateMockDataCommand,
        importTpsCsvCommand,
    };

    return rootCommand;
}

static Command CreateMigrateDatabaseCommand()
{
    var migrateDatabaseCommand = new Command("migratedb", "Ensure that any Entity Framework migrations are executed in order to create the database and ensure the latest updates are applied.")
    {
    };

    migrateDatabaseCommand.SetHandler(MigrateDatabase);

    return migrateDatabaseCommand;
}

static async Task MigrateDatabase()
{
    var configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();
#if DEBUG
    configBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
#endif

    var config = configBuilder.Build();
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(config);
    services.AddDbContext<WorkforceDbContext>();
    var sp = services.BuildServiceProvider();

    await using var scope = sp.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await dbContext.Database.MigrateAsync();
}

static Command CreateGenerateMockDataCommand()
{   
    var directoryOption = new Option<string>(
        name: "--directory",
        description: "Directory where files will be written.");
    directoryOption.AddAlias("-d");

    var monthsOption = new Option<int>(
        name: "--months",
        description: "The number of months worth of data to generate.",
        getDefaultValue: () => 12);

    var teachersOption = new Option<int>(
        name: "--teachers",
        description: "The number of teachers to generate mock data for.",
        getDefaultValue: () => 700_000);

    var percentChangeSchoolsOnceOption = new Option<int>(
        name: "--percent-change-schools-once",
        description: "The % of teachers to generate who change schools once during the months we're generating data for.",
        getDefaultValue: () => 9);

    var generateMockDataCommand = new Command("generatemockdata", "Generate test data in the format expected from the TPS extract.")
    {
        directoryOption,
        monthsOption,
        teachersOption
    };

    generateMockDataCommand.SetHandler(
        GenerateMockData,
        directoryOption,
        monthsOption,
        teachersOption);

    return generateMockDataCommand;
}

static Command CreateImportTpsCsvCommand()
{
    var filenameOption = new Option<string>(
        name: "--filename",
        description: "Filename of TPS extract CSV to import.",
        getDefaultValue: () => "tps-extract.csv");
    filenameOption.AddAlias("-f");

    var insertTpsCsvCommand = new Command("importtpscsv", "Process TPS extract CSV file and insert data into the database.")
    {
        filenameOption
    };

    insertTpsCsvCommand.SetHandler(
        ImportTpsCsv,
        filenameOption);

    return insertTpsCsvCommand;
}

static async Task ImportTpsCsv(string filename)
{
    var configBuilder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
#if DEBUG
    configBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
#endif

    var config = configBuilder.Build();

    var serilog = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();

    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(config);
    services.AddLogging(builder => builder.AddSerilog(serilog));
    services.AddDbContext<WorkforceDbContext>();
    services.AddSingleton<ITpsCsvProcessor, TpsCsvProcessor>();
    var sp = services.BuildServiceProvider();

    var tpsCsvProcessor = sp.GetRequiredService<ITpsCsvProcessor>();
    await tpsCsvProcessor.Process(filename);
    Console.WriteLine("Imported CSV");
}

static void GenerateMockData(
    string directory,
    int months,
    int teachers)
{
    var configBuilder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
#if DEBUG
    configBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
#endif

    var config = configBuilder.Build();

    var serilog = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();

    var services = new ServiceCollection();
    services.AddSingleton<IEstablishmentGenerationService, EstablishmentGenerationService>();
    services.AddLogging(builder => builder.AddSerilog(serilog));
    services.AddSingleton<TestDataGenerator>();
    var sp = services.BuildServiceProvider();

    var testDataGenerator = sp.GetRequiredService<TestDataGenerator>();
    var testData = testDataGenerator.GenerateTestData(
        DateOnly.FromDateTime(DateTime.Today.AddYears(-1).AddMonths(-1)),
        months,
        teachers);

    var logger = sp.GetRequiredService<ILogger<Program>>();

    string dateTimeSuffix = DateTime.Now.ToString("ddMMyyyyHHmmss");
    string identityUsersCsvFilename = $"mock-teacher-identity-users-{dateTimeSuffix}.csv";
    string tpsExtractCsvFilename = $"mock-tps-extract-{dateTimeSuffix}.csv";
    var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true
    };     

    using var identityUsersWriter = new StreamWriter(identityUsersCsvFilename);
    using var identityUsersCsv = new CsvWriter(identityUsersWriter, csvConfiguration);
    identityUsersCsv.Context.RegisterClassMap<TeacherIdentityUserWriterMap>();

    using var tpsExtractWriter = new StreamWriter(tpsExtractCsvFilename);
    using var tpsExtractCsv = new CsvWriter(tpsExtractWriter, csvConfiguration);
    tpsExtractCsv.Context.RegisterClassMap<TpsExtractDataItemWriterMap>();
    int teacherCount = 0;
    int workforceDataItemsCount = 0;
    foreach (var workforceData in testData)
    {
        var identityUser = MapTeacherToIdentityUser(workforceData.Teacher);
        identityUsersCsv.WriteRecords(new[] { identityUser });
        teacherCount++;
        tpsExtractCsv.WriteRecords(workforceData.WorkforceDataItems);
        workforceDataItemsCount += workforceData.WorkforceDataItems.Length;

        if (teacherCount != 0 && teacherCount % 50_000 == 0)
        {
            logger.LogInformation("Generated {teacherCount} teacher identity users.", teacherCount);
            logger.LogInformation("Generated {workforceDataItemsCount} workforce data items.", workforceDataItemsCount);
        }
    }

    // Log any unlogged totals
    if (teacherCount % 50_000 != 0)
    {
        logger.LogInformation("Generated {teacherCount} teacher identity users.", teacherCount);
        logger.LogInformation("Generated {workforceDataItemsCount} workforce data items.", workforceDataItemsCount);
    }    
}

static User MapTeacherToIdentityUser(Teacher teacher)
{
    var identityUser = new User
    {
        UserId = Guid.NewGuid(),
        EmailAddress = teacher.EmailAddress,
        FirstName = teacher.FirstName,
        LastName = teacher.LastName,
        DateOfBirth = teacher.DateOfBirth,
        UserType = UserType.Teacher,
        Trn = teacher.Trn
    };

    return identityUser; 
}
