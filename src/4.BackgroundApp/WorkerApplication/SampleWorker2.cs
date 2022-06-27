using Application.Base;
using eXtensionSharp;
using TodoService.Services;

namespace WorkerApplication;

public class SampleWorker2 : ParallelBackgroundServiceBase<Domain.Entities.TB_TODO>
{
    public SampleWorker2(ILogger<SampleWorker2> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) 
        : base(logger, configuration, serviceScopeFactory)
    {
    }

    protected override async Task<IEnumerable<Domain.Entities.TB_TODO>> OnProducerAsync(CancellationToken stopingToken)
    {
        IEnumerable<Domain.Entities.TB_TODO> todos = null;
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ITodoService>();

            todos = await service.GetAllTodoByDateAsync(DateTime.Now);
        }

        return todos;
    }

    protected override async Task OnConsumerAsync(Domain.Entities.TB_TODO item, CancellationToken stopingToken)
    {
        await Task.Factory.StartNew(() =>
        {
            Console.WriteLine(item.CONTENTS);
        });
    }

    protected override Task OnFinishAsync(IEnumerable<Domain.Entities.TB_TODO> items, CancellationToken stopingToken)
    {
        if (items.xIsEmpty())
        {
            Console.WriteLine("item is empty.");
        }
        else
        {
            Console.WriteLine($"item is not empty : {items.xCount()}");
        }

        return Task.CompletedTask;
    }
}