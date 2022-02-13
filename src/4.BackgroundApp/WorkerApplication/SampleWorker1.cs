using Application.Abstract;
using eXtensionSharp;
using TodoService.Services;

namespace WorkerApplication;

public class SampleWorker1 : BackgroundServiceBase
{
   public SampleWorker1(ILogger<SampleWorker1> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory)
    {
        _interval = int.Parse(configuration["SampleWorer1Options:Interval"]) * 1000;
    }

    protected override async Task ExecuteCore(CancellationToken stopingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ITodoService>();

            var todos = await service.GetAllTodoByDateAsync(DateTime.Now);
            
            todos.xForEach(todo =>
            {
                Console.WriteLine(todo.xToJson());
            });
        }
    }
}