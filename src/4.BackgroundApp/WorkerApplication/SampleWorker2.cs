using Application.Abstract;

namespace WorkerApplication;

public class SampleWorker2 : BackgroundServiceBase
{
    public SampleWorker2(ILogger<SampleWorker2> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory)
    {
    }

    protected override Task OnRunAsync(CancellationToken stopingToken)
    {
        _logger.LogInformation("run executecore");
        return Task.CompletedTask;
    }
}