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
using WorkforceDataApi.Services;

var rootCommand = CreateRootCommand();
return await rootCommand.InvokeAsync(args);

static RootCommand CreateRootCommand()
{
    var migrateDatabaseCommand = CreateMigrateDatabaseCommand();
    var generateMockDataCommand = CreateGenerateMockDataCommand();    
    var importTpsCsvCommand = CreateImportTpsCsvCommand();
    var importEstablishmentsCsvCommand = CreateImportEstablishmentsCsv();
    var azureBlobCommand = CreateAzureBlobCommand();

    var rootCommand = new RootCommand("Workforce Data command line developer utilities.")
    {
        migrateDatabaseCommand,
        generateMockDataCommand,        
        importTpsCsvCommand,
        importEstablishmentsCsvCommand,
        azureBlobCommand
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

    var percentSupplyTeachersOption = new Option<int>(
        name: "--percent-supply-teachers",
        description: "The % of teachers are supply teachers working in up to 3 schools per month during the months we're generating data for.",
        getDefaultValue: () => 1);

    var percentNewStartersOption = new Option<int>(
        name: "--percent-new-starters",
        description: "The % of teachers start teaching during the months we're generating data for.",
        getDefaultValue: () => 10);

    var percentLeaversOption = new Option<int>(
        name: "--percent-leavers",
        description: "The % of teachers leaving teaching during the months we're generating data for.",
        getDefaultValue: () => 8);

    var percentPartTimeOption = new Option<int>(
        name: "--percent-part-time",
        description: "The % of teachers who work part-time during the months we're generating data for.",
        getDefaultValue: () => 10);

    var generateMockDataCommand = new Command("generatemockdata", "Generate test data in the format expected from the TPS extract.")
    {
        directoryOption,
        monthsOption,
        teachersOption,
        percentChangeSchoolsOnceOption,
        percentSupplyTeachersOption,
        percentNewStartersOption,
        percentLeaversOption,
        percentPartTimeOption
    };

    generateMockDataCommand.SetHandler(
        GenerateMockData,
        directoryOption,
        monthsOption,
        teachersOption,
        percentChangeSchoolsOnceOption,
        percentSupplyTeachersOption,
        percentNewStartersOption,
        percentLeaversOption,
        percentPartTimeOption);

    return generateMockDataCommand;
}

static void GenerateMockData(
    string directory,
    int months,
    int teachers,
    int changeJobOncePercentage,
    int supplyTeacherPercentage,
    int newStarterPercentage,
    int leaverPercentage,
    int partTimePercentage)
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
        teachers,
        changeJobOncePercentage,
        supplyTeacherPercentage,
        newStarterPercentage,
        leaverPercentage,
        partTimePercentage);

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

    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Imported TPS Extract CSV");
}

static Command CreateImportEstablishmentsCsv()
{
    var filenameOption = new Option<string>(
        name: "--filename",
        description: "Filename of Establishments CSV to import.",
        getDefaultValue: () => "edubasealldata20230119.csv");
    filenameOption.AddAlias("-f");

    var downloadLatestOption = new Option<bool>(
        name: "--download-latest",
        description: "Specifies whether to download the latest Establishments CSV file to use in the import.",
        getDefaultValue: () => true);

    var importEstablishmentsCsvCommand = new Command("importestablishmentscsv", "Process Establishments CSV file and insert data into the database.")
    {
        filenameOption,
        downloadLatestOption
    };

    importEstablishmentsCsvCommand.SetHandler(
        ImportEstablishmentsCsv,
        filenameOption,
        downloadLatestOption);

    return importEstablishmentsCsvCommand;
}

static async Task ImportEstablishmentsCsv(
    string filename,
    bool downloadLatest)
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
    services.AddSingleton<IEstablishmentsCsvProcessor, EstablishmentsCsvProcessor>();
    if (downloadLatest)
    {
        services.AddHttpClient();
        services.AddSingleton<IEstablishmentsCsvDownloader, EstablishmentsCsvDownloader>();
    }
    
    var sp = services.BuildServiceProvider();
    var logger = sp.GetRequiredService<ILogger<Program>>();

    if (downloadLatest)
    {
        logger.LogInformation("Downloading latest Establishments CSV");
        var establishmentsCsvDownloader = sp.GetRequiredService<IEstablishmentsCsvDownloader>();
        filename = await establishmentsCsvDownloader.DownloadLatest();
        logger.LogInformation("Downloaded {filename}", filename);
    }

    var establishmentsCsvProcessor = sp.GetRequiredService<IEstablishmentsCsvProcessor>();
    await establishmentsCsvProcessor.Process(filename);
    logger.LogInformation("Imported Establishments CSV");
}

static Command CreateAzureBlobCommand()
{
    var listPendingFilesCommand = CreateListPendingFilesCommand();
    var downloadPendingFilesCommand = CreateDownloadPendingFilesCommand();
    var archivePendingFilesCommand = CreateArchivePendingFilesCommand();

    var azureBlobCommand = new Command("azblob", "Commands related to interacting with Azure blob storage.")
    {
        listPendingFilesCommand,
        downloadPendingFilesCommand,
        archivePendingFilesCommand
    };    

    return azureBlobCommand;
}

static Command CreateListPendingFilesCommand()
{
    var listPendingFilesCommand = new Command("listpending", "Lists the TPS extract files pending processing.")
    {
    };

    listPendingFilesCommand.SetHandler(ListPendingFiles);

    return listPendingFilesCommand;
}

static async Task ListPendingFiles()
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
    services.AddSingleton<ICloudStorageService, AzureBlobStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ICloudStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames();
    if (filenames == null || filenames.Length == 0)
    {
        logger.LogInformation("No pending TPS extract files found.");
    }
    else
    {
        logger.LogInformation("Found the following pending TPS extract files.");
        foreach (var filename in filenames)
        {
            logger.LogInformation("{filename}", filename);
        }
    }
}

static Command CreateDownloadPendingFilesCommand()
{
    var downloadPendingFilesCommand = new Command("downloadpending", "Downloads all TPS extract files pending processing.")
    {
    };

    downloadPendingFilesCommand.SetHandler(DownloadPending);

    return downloadPendingFilesCommand;
}

static async Task DownloadPending()
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
    services.AddSingleton<ICloudStorageService, AzureBlobStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ICloudStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames();
    if (filenames == null || filenames.Length == 0)
    {
        logger.LogInformation("No pending TPS extract files found.");
    }
    else
    {        
        foreach (var filename in filenames)
        {
            logger.LogInformation("Downloading {filename}", filename);
            await storageService.DownloadTpsExtractFile(filename);
            logger.LogInformation("Done.");
        }
    }
}

static Command CreateArchivePendingFilesCommand()
{
    var archivePendingFilesCommand = new Command("archivepending", "Archives all TPS extract files pending processing.")
    {
    };

    archivePendingFilesCommand.SetHandler(ArchivePending);

    return archivePendingFilesCommand;
}

static async Task ArchivePending()
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
    services.AddSingleton<ICloudStorageService, AzureBlobStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ICloudStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames();
    if (filenames == null || filenames.Length == 0)
    {
        logger.LogInformation("No pending TPS extract files found.");
    }
    else
    {
        foreach (var filename in filenames)
        {
            logger.LogInformation("Archiving {filename}", filename);
            await storageService.ArchiveTpsExtractFile(filename);
            logger.LogInformation("Done.");
        }
    }
}
