namespace WorkerApplication;

public class SampleWorker2 : WorkerServiceBase
{
    public SampleWorker2(ILogger<SampleWorker2> logger) : base(logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("executeasync sampleworker2");
            await Task.Delay(1000, stoppingToken);
        }
        await base.ExecuteAsync(stoppingToken);
    }
}