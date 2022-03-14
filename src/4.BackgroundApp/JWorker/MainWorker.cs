using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstract;
using Application.Script.SharpScript;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JWorker;

public class MainWorker : BackgroundServiceBase
{
    private readonly SharpScriptLoader _sharpScriptLoader;
    private readonly List<ISharpScriptBase> _scripters = new List<ISharpScriptBase>();
    private bool _isRunning = false; 

    public MainWorker(ILogger<MainWorker> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, SharpScriptLoader sharpScriptLoader)
        : base(logger, configuration, serviceScopeFactory, 10)
    {
        _sharpScriptLoader = sharpScriptLoader;
        _scripters.Add(_sharpScriptLoader.Create("Sample1.cs").AddExecutor<string, string>(string.Empty, string.Empty, new[]
        {
            "System", "System.Threading.Tasks", "Microsoft.Extensions.DependencyInjection", "Microsoft.Extensions.Logging", "Application.Script.SharpScript",
        }));
        _scripters.Add(_sharpScriptLoader.Create("Sample2.cs").AddExecutor<string, string>(string.Empty, string.Empty,new[]
        {
            "System", "System.Threading.Tasks", "Microsoft.Extensions.DependencyInjection", "Microsoft.Extensions.Logging", "Application.Script.SharpScript",
        }));
    }

    protected override async Task OnRunAsync(CancellationToken stopingToken)
    {
        if (_isRunning.xIsFalse())
        {
            _isRunning = true;
            await Parallel.ForEachAsync(_scripters, async (scripter, token) =>
            {
                await scripter.ExecuteAsync();
            });
            _isRunning = false;
        }
    }
}