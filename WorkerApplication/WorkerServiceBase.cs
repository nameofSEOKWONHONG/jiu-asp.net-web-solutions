using eXtensionSharp;

namespace WorkerApplication;

public abstract class WorkerServiceBase : BackgroundService
{
    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration = null!;

    protected WorkerServiceBase(ILogger logger)
    {
        this._logger = logger;
    }

    protected WorkerServiceBase(ILogger logger, IConfiguration configuration) : this(logger)
    {
        this._configuration = configuration;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"StartAsync : {DateTime.Now.ToShortTimeString()}");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"StopAsync :  {DateTime.Now.ToShortTimeString()}");
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"ExecuteAsync : {DateTime.Now.ToShortTimeString()}");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}