using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sentry.AspNetCore;
using Serilog;
using WorkforceDataApi.Models;
using WorkforceDataApi.Responses;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddApplicationInsightsTelemetry();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseSentry();
    builder.Services.Configure<SentryAspNetCoreOptions>(options =>
    {
        var hostingEnvironmentName = builder.Configuration["EnvironmentName"];
        if (!string.IsNullOrEmpty(hostingEnvironmentName))
        {
            options.Environment = hostingEnvironmentName;
        }

        var gitSha = builder.Configuration["GitSha"];
        if (!string.IsNullOrEmpty(gitSha))
        {
            options.Release = gitSha;
        }
    });
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "TRN Generation API", Version = "v1" });
});
builder.Services.AddDbContext<WorkforceDbContext>();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration
        .AddUserSecrets<Program>();
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsProduction() &&
    Environment.GetEnvironmentVariable("WEBSITE_ROLE_INSTANCE_ID") == "0")
{
    await MigrateDatabase();
}

app.UseSwagger();

if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.EnablePersistAuthorization();
    });
}

app.MapGet("/api/v1/tps-extract-data-items", async (int trn, WorkforceDbContext dbContext) =>
    {
        var items = await dbContext.TpsExtractDataItems
            .Where(i => i.Trn == trn.ToString())
            .Select(i => i.Adapt<GetTpsExtractDataItemResponseBody>())
            .ToListAsync();

        var response = new GetTpsExtractDataItemsResponse
        {
            TpsExtractDataItems = items
        };

        return Results.Ok(response);
    })
    .WithTags("TPS Extract")
    .Produces<GetTpsExtractDataItemsResponse>(StatusCodes.Status200OK);

app.MapGet("/api/v1/tps-members", async (int trn, WorkforceDbContext dbContext) =>
{
    var member = await dbContext.TpsExtractDataItems
        .Where(i => i.Trn == trn.ToString())
        .FirstOrDefaultAsync(); 

    if (member == null)
    {
        return Results.NotFound();
    }

    var response = new GetTpsMemberResponse
    {
        MemberId = member.MemberId,
    };

    return Results.Ok(response);
})
    .WithTags("TPS Extract")
    .Produces<GetTpsExtractDataItemsResponse>(StatusCodes.Status200OK);

app.MapGet("/api/v1/tps-extract-data-items", async (string memberId, WorkforceDbContext dbContext) =>
{
    var items = await dbContext.TpsExtractDataItems
        .Where(i => i.MemberId == memberId)
        .Select(i => i.Adapt<GetTpsExtractDataItemResponseBody>())
        .ToListAsync();

    var response = new GetTpsExtractDataItemsResponse
    {
        TpsExtractDataItems = items
    };

    return Results.Ok(response);
})
    .WithTags("TPS Extract")
    .Produces<GetTpsExtractDataItemsResponse>(StatusCodes.Status200OK);

app.MapGet("/health", () => Results.Ok("OK"))
    .WithTags("Health");

app.Run();

async Task MigrateDatabase()
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await dbContext.Database.MigrateAsync();
}

public partial class Program { }
