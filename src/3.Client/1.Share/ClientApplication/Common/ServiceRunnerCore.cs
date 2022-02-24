using eXtensionSharp;

namespace ClientApplication.Common.Services;

public class ServiceRunnerCore : IDisposable
{
    private readonly IList<IServiceRunnerBase> _runnerItems = new List<IServiceRunnerBase>();
    public ServiceRunnerCore(IServiceRunnerBase rootRunner, IRunnerOptions options = null)
    {
        if (options.xIsNotEmpty())
        {
            rootRunner.Options = options;
            if (rootRunner.Options.Request.xIsEmpty())
                rootRunner.Options.Request = new DynamicDictionary<string>();
            if (rootRunner.Options.Result.xIsEmpty())
                rootRunner.Options.Result = new DynamicDictionary<string>();
        }
        _runnerItems.Add(rootRunner);
    }

    public async Task RunAsync()
    {
        var i = 0;
        while (true)
        {
            if (!await _runnerItems[i].OnExecuteAsync())
            {
                //current fail process
                await _runnerItems[i].OnErrorAsync();
                //previous fail process
                for (var j = i - 1; j >= 0; j--)
                {
                    await _runnerItems[j].OnErrorAsync();    
                }
                break;
            }    
            if(_runnerItems[i].Next.xIsEmpty()) break;
            _runnerItems[i].Next.Options = new RunnerOptions();
            _runnerItems[i].Next.Options.Result = new DynamicDictionary<string>();
            _runnerItems[i].Next.Options.Request = _runnerItems[i].Options.Result;
            _runnerItems.Add(_runnerItems[i].Next);
            i++;
        }
    }

    public void Dispose()
    {
        _runnerItems.xForEach(item =>
        {
            item.Dispose();
        });
    }
}

public class ServiceParallelRunnerCore : IDisposable
{
    private readonly IEnumerable<IServiceRunnerBase> _runners;
    private readonly int _maxDegreeOfParallelism;
    public ServiceParallelRunnerCore(IEnumerable<IServiceRunnerBase> runners, int maxDegreeOfParallelism = 10)
    {
        _runners = runners;
        _maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Run ServiceParallelRunnerCore");
        await _runners.xForEachParallelAsync(async (runner, cancellationToken) =>
        {
            runner.Options.Request.xIfEmpty(() => runner.Options.Request = new DynamicDictionary<string>());
            runner.Options.Result.xIfEmpty(() =>
                runner.Options.Result = new DynamicDictionary<string>());
            if (!await runner.OnExecuteAsync())
            {
                await runner.OnErrorAsync();
            }
        }, new ParallelOptions(){MaxDegreeOfParallelism = _maxDegreeOfParallelism});
        Console.WriteLine("End ServiceParallelRunnerCore");
    }
    
    public void Dispose()
    {
        
    }
}
