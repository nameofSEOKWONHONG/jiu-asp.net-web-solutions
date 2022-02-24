using eXtensionSharp;

namespace ClientApplication.Common.Services;

public interface IRunnerOptions
{
    DynamicDictionary<string> Request { get; set; }
    DynamicDictionary<string> Result { get; set; }
}

public class RunnerOptions : IRunnerOptions
{
    public DynamicDictionary<string> Request { get; set; }
    public DynamicDictionary<string> Result { get; set; }
} 

public class RestServiceRunnerCore : IDisposable
{
    private readonly IList<IRestServiceRunnerBase> _runnerItems = new List<IRestServiceRunnerBase>();
    public RestServiceRunnerCore(IRestServiceRunnerBase rootRunner, IRunnerOptions options = null)
    {
        _runnerItems.Add(rootRunner);
        if (options.xIsNotEmpty())
        {
            _runnerItems[0].Options = options;
            if (_runnerItems[0].Options.Request.xIsEmpty())
                _runnerItems[0].Options.Request = new DynamicDictionary<string>();
            if (_runnerItems[0].Options.Result.xIsEmpty())
                _runnerItems[0].Options.Result = new DynamicDictionary<string>();
        }
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