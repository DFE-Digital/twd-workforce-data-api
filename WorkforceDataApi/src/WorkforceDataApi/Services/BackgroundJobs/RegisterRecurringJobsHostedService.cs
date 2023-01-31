using Hangfire;

namespace WorkforceDataApi.Services.BackgroundJobs;

public class RegisterRecurringJobsHostedService : IHostedService
{
    private readonly IRecurringJobManager _recurringJobManager;

    public RegisterRecurringJobsHostedService(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RegisterJobs();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void RegisterJobs()
    {
        _recurringJobManager.AddOrUpdate<TpsExtractJob>(nameof(TpsExtractJob), job => job.Execute(CancellationToken.None), Cron.MinuteInterval(2));
    }
}