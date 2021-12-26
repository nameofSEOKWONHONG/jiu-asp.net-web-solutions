using Application.Interfaces.Todo;
using eXtensionSharp;

namespace WorkerApplication;

public class SampleWorker1 : WorkerServiceBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _interval = 0;
    
    public SampleWorker1(ILogger<SampleWorker1> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory) : base(logger, configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _interval = int.Parse(configuration["SampleWorer1Options:Interval"]) * 1000;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("executeasync sampleworker1");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ITodoService>();

                var todos = await service.GetAllTodoByDateAsync(DateTime.Now);
            
                todos.xForEach(todo =>
                {
                    Console.WriteLine(todo.xToJson());
                });
            }
            
            await Task.Delay(_interval, stoppingToken);
        }
        await base.ExecuteAsync(stoppingToken);
    }
}