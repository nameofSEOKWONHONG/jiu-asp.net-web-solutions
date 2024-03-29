﻿using Application.Base;
using eXtensionSharp;
using TodoService.Services;

namespace WorkerApplication;

public class SampleWorker1 : BackgroundServiceBase
{
   public SampleWorker1(ILogger<SampleWorker1> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory, 60, "000000", "060000")
    {
        _interval = int.Parse(configuration["SampleWorer1Options:Interval"]) * 1000;
    }

    protected override async Task OnRunAsync(CancellationToken stopingToken)
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