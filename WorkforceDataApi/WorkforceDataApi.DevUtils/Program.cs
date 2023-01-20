using System.CommandLine;
using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkforceDataApi.Csv;
using WorkforceDataApi.DevUtils.Csv;
using WorkforceDataApi.DevUtils.Services;
using WorkforceDataApi.Models;

var rootCommand = CreateRootCommand();
return await rootCommand.InvokeAsync(args);

static RootCommand CreateRootCommand()
{
    var migrateDatabaseCommand = CreateMigrateDatabaseCommand();
    var generateTestCsvCommand = CreateGenerateTestCsvCommand();
    var insertTestDataCommand = CreateInsertTestDataCommand();

    var rootCommand = new RootCommand("Workforce Data command line developer utilities.")
    {
        migrateDatabaseCommand,
        generateTestCsvCommand,
        insertTestDataCommand,
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

static Command CreateGenerateTestCsvCommand()
{
    var generateTestCsvCommand = new Command("generatetestcsv", "Generate test data and write it out to a CSV file in the format expected from the TPS extract.")
    {
    };

    generateTestCsvCommand.SetHandler(GenerateTestCsv);

    return generateTestCsvCommand;
}

static Command CreateInsertTestDataCommand()
{
    var insertTestDataCommand = new Command("inserttestdata", "Generate test data and insert it into the database.")
    {
    };

    insertTestDataCommand.SetHandler(InsertTestData);

    return insertTestDataCommand;
}

static async Task InsertTestData()
{
    var configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();
#if DEBUG
    configBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
#endif

    var config = configBuilder.Build();
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(config);
    services.AddLogging();
    services.AddDbContext<WorkforceDbContext>();
    services.AddSingleton<ITpsCsvProcessor, TpsCsvProcessor>();
    var sp = services.BuildServiceProvider();

    var tpsCsvProcessor = sp.GetRequiredService<ITpsCsvProcessor>();
    await tpsCsvProcessor.Process("testtpsextract.csv");
    Console.WriteLine("Imported CSV");
}

static void GenerateTestCsv()
{
    var testDataGenerator = new TestDataGenerator();
    var testData = testDataGenerator.GenerateTestData(DateOnly.FromDateTime(DateTime.Today.AddYears(-1).AddMonths(-1)), teacherCount: 100000);

    using var writer = new StreamWriter("testtpsextract.csv");
    using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
    csv.Context.RegisterClassMap<TpsExtractDataItemWriterMap>();
    foreach (var item in testData)
    {
        csv.WriteRecords(new[] { item });        
    }
}

static void GenerateTestCsvToConsole()
{
    var testDataGenerator = new TestDataGenerator();
    var testData = testDataGenerator.GenerateTestData(DateOnly.FromDateTime(DateTime.Today));

    using var writer = new StreamWriter(Console.OpenStandardOutput());
    using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
    csv.Context.RegisterClassMap<TpsExtractDataItemWriterMap>();
    foreach (var item in testData)
    {
        csv.WriteRecords(new[] { item });
    }
}

static void ReadTestCsvToConsole()
{
    using var reader = new StreamReader("testtpsextract.csv");
    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
    csv.Context.RegisterClassMap<TpsExtractDataItemReaderMap>();
    var records = csv.GetRecords<TpsExtractDataItem>();
    foreach (var item in records)
    {
        var json = JsonSerializer.Serialize(item);
        Console.WriteLine(json);
    }
}
