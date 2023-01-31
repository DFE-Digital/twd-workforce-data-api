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
using WorkforceDataApi.Services.BackgroundJobs;

var rootCommand = CreateRootCommand();
return await rootCommand.InvokeAsync(args);

static RootCommand CreateRootCommand()
{
    var migrateDatabaseCommand = CreateMigrateDatabaseCommand();
    var generateMockDataCommand = CreateGenerateMockDataCommand();
    var importTpsCsvCommand = CreateImportTpsCsvCommand();
    var execJobCommand = CreateExecJobCommand();
    var importEstablishmentsCsvCommand = CreateImportEstablishmentsCsvCommand();
    var azureBlobCommand = CreateAzureBlobCommand();

    var rootCommand = new RootCommand("Workforce Data command line developer utilities.")
    {
        migrateDatabaseCommand,
        generateMockDataCommand,        
        importTpsCsvCommand,
        execJobCommand,
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

    await WithDbContext(dbContext => dbContext.Database.MigrateAsync(), sp);
}

static Command CreateGenerateMockDataCommand()
{
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
        monthsOption,
        teachersOption,
        percentChangeSchoolsOnceOption,
        percentSupplyTeachersOption,
        percentNewStartersOption,
        percentLeaversOption,
        percentPartTimeOption);

    return generateMockDataCommand;
}

static async Task GenerateMockData(
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
    services.AddHttpClient();
    services.AddSingleton<IEstablishmentsCsvDownloader, EstablishmentsCsvDownloader>();
    services.AddSingleton<IEstablishmentGenerationService, EstablishmentGenerationService>();
    services.AddLogging(builder => builder.AddSerilog(serilog));
    services.AddSingleton<TestDataGenerator>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var establishmentsCsvDownloader = sp.GetRequiredService<IEstablishmentsCsvDownloader>();
    var latestEstablishmentsCsvFilename = establishmentsCsvDownloader.GetLatestEstablishmentsCsvFilename();
    if (!File.Exists(latestEstablishmentsCsvFilename))
    {
        logger.LogInformation("Downloading latest Establishments CSV");
        await establishmentsCsvDownloader.DownloadLatest();
        logger.LogInformation("Downloaded {filename}", latestEstablishmentsCsvFilename);
    }

    var establishmentGenerationService = sp.GetRequiredService<IEstablishmentGenerationService>();
    establishmentGenerationService.Initialise(latestEstablishmentsCsvFilename);

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
    logger.LogInformation("Generating mock TPS extract data to {tpsCsv}.", tpsExtractCsvFilename);
    logger.LogInformation("Generating mock teacher identity users to {usersCsv}.", identityUsersCsvFilename);
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

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var tpsCsvProcessor = sp.GetRequiredService<ITpsCsvProcessor>();

    logger.LogInformation("Importing TPS Extract from {filename}", filename);
    await tpsCsvProcessor.Process(filename);    
    logger.LogInformation("Imported {filename}", filename);
}

static Command CreateExecJobCommand()
{
    var jobOption = new Option<string>(
        name: "--job",
        description: "Name of job to execute.",
        getDefaultValue: () => "tps-extract")
        .FromAmong("tps-extract");
    var overwriteExistingOption = new Option<bool>(
        name: "--overwrite-existing",
        description: "Specifies whether to overwrite existing data when executing the job.",
        getDefaultValue: () => true);

    var execJobCommand = new Command("execjob", "Execute background job.")
    {
        jobOption,
        overwriteExistingOption
    };

    execJobCommand.SetHandler(
        ExecJob,
        jobOption,
        overwriteExistingOption);

    return execJobCommand;
}

static async Task ExecJob(
    string jobName,
    bool overwriteExisting)
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
    services.AddSingleton<ILocalFilesystem, LocalFilesystem>();
    services.AddSingleton<ITpsExtractRemoteStorageService, AzureTpsExtractRemoteStorageService>();
    services.AddSingleton<ITpsCsvProcessor, TpsCsvProcessor>();
    services.AddSingleton<TpsExtractJob>();
    
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();

    if (jobName == "tps-extract")
    {
        if (overwriteExisting)
        {
            logger.LogInformation("Clearing down existing TPS Extract data.");
            await WithDbContext(dbContext => dbContext.Database.ExecuteSqlRawAsync("Truncate table tps_extract_data_item"), sp);
            logger.LogInformation("Done.");
        }

        logger.LogInformation("Executing TPS Extract Job.");
        var tpsExtractJob = sp.GetRequiredService<TpsExtractJob>();
        await tpsExtractJob.Execute(CancellationToken.None);
        logger.LogInformation("Finished TPS Extract Job.");
    }
    else
    {
        logger.LogWarning("No jobs to execute.");
    }
}

static Command CreateImportEstablishmentsCsvCommand()
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
    services.AddSingleton<ITpsExtractRemoteStorageService, AzureTpsExtractRemoteStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ITpsExtractRemoteStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames(CancellationToken.None);
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
    services.AddSingleton<ITpsExtractRemoteStorageService, AzureTpsExtractRemoteStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ITpsExtractRemoteStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames(CancellationToken.None);
    if (filenames == null || filenames.Length == 0)
    {
        logger.LogInformation("No pending TPS extract files found.");
    }
    else
    {
        var basePath = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData);
        var downloadFolder = Path.Combine(basePath, "tps-extract-download");

        foreach (var filename in filenames)
        {
            logger.LogInformation("Downloading {filename}", filename);
            await storageService.DownloadTpsExtractFile(filename, downloadFolder, CancellationToken.None);
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
    services.AddSingleton<ITpsExtractRemoteStorageService, AzureTpsExtractRemoteStorageService>();
    var sp = services.BuildServiceProvider();

    var logger = sp.GetRequiredService<ILogger<Program>>();
    var storageService = sp.GetRequiredService<ITpsExtractRemoteStorageService>();
    var filenames = await storageService.GetPendingProcessingTpsExtractFilenames(CancellationToken.None);
    if (filenames == null || filenames.Length == 0)
    {
        logger.LogInformation("No pending TPS extract files found.");
    }
    else
    {
        foreach (var filename in filenames)
        {
            logger.LogInformation("Archiving {filename}", filename);
            await storageService.ArchiveTpsExtractFile(filename, CancellationToken.None);
            logger.LogInformation("Done.");
        }
    }
}

static async Task WithDbContext(Func<WorkforceDbContext, Task> action, IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await action(dbContext);
}
