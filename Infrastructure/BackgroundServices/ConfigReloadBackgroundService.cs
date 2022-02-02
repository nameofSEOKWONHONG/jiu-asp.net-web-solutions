using Application.Abstract;
using Application.Script;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Application.Script.PyScript;
using Domain.Configuration;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class ConfigReloadBackgroundService : BackgroundServiceBase
{
    private static object _syncObj = new object();
    
    public ConfigReloadBackgroundService(ILogger<ConfigReloadBackgroundService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory)
    {
    }

    private readonly Dictionary<ENUM_SCRIPT_TYPE, Func<IServiceScope, IScriptReset>> _resetStates =
        new Dictionary<ENUM_SCRIPT_TYPE, Func<IServiceScope, IScriptReset>>()
        {
            {
                ENUM_SCRIPT_TYPE.CSHARP, scope => scope.ServiceProvider.GetService<SharpScriptLoader>()
            },
            {
                ENUM_SCRIPT_TYPE.JS, scope => scope.ServiceProvider.GetService<JsScriptLoader>()
            },
            {
                ENUM_SCRIPT_TYPE.PY, scope => scope.ServiceProvider.GetService<PyScriptLoader>()
            }
        };

    protected override async Task ExecuteCore(CancellationToken stoppingToken)
    {
        lock (_syncObj)
        {
            using (var scope = this._serviceScopeFactory.CreateScope())
            {
                //IOptions<>는 AutoReload를 지원하지 않는다.
                //config AutoReload를 사용하려면 IOptionsMonitor를 사용해야 함.
                //또한 config AutoReload는 Program.cs > CreateHostBuilder 부분을 확인하면 된다. 
                var resetConfig = scope.ServiceProvider.GetService<IOptionsMonitor<ScriptLoaderConfig>>();
                var scriptInitializer = scope.ServiceProvider.GetService<ScriptInitializer>();

                try
                {
                    resetConfig.CurrentValue.ResetFileConfigs.xForEach(config =>
                    {
                        var getScriptLoader = _resetStates[config.ScriptType];
                        var scriptLoader = getScriptLoader(scope);
                        scriptInitializer.Reset(scriptLoader, resetConfig.CurrentValue.Version, config.ResetFiles);  
                    });
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, e.Message);
                }
            }
        }
    }
}