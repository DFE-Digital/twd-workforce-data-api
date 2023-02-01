using Hangfire;

namespace WorkforceDataApi.Services.BackgroundJobs;

public class RegisterRecurringJobsHostedService : IHostedService
{
    private const string DefaultTpsExtractJobSchedule = "0 0 * * *"; // Midnight daily
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<RegisterRecurringJobsHostedService> _logger;
    private string _tpsExtractJobSchedule;

    public RegisterRecurringJobsHostedService(
        IConfiguration configuration,
        IRecurringJobManager recurringJobManager,
        ILogger<RegisterRecurringJobsHostedService> logger)
    {
        _tpsExtractJobSchedule = configuration.GetValue("TpsExtractJobSchedule", DefaultTpsExtractJobSchedule) ?? DefaultTpsExtractJobSchedule;
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RegisterJobs();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void RegisterJobs()
    {
        _logger.LogInformation("Adding recurring TPS extract job with CRON expression {schedule}", _tpsExtractJobSchedule);
        _recurringJobManager.AddOrUpdate<TpsExtractJob>(nameof(TpsExtractJob), job => job.Execute(CancellationToken.None), _tpsExtractJobSchedule);
    }
}
