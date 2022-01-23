using Application.Script;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Application.Script.PyScript;
using Domain.Configuration;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class ConfigReloadBackgroundService : BackgroundServiceBase
{
    /// <summary>
    /// interval (default : 5sec)
    /// </summary>
    private readonly int _interval = 1000 * 5;

    private static object _syncObj = new object();
    
    public ConfigReloadBackgroundService(ILogger<ConfigReloadBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory)
    {
        this._serviceScopeFactory = serviceScopeFactory;
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

    protected override async Task Execute(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ConfigReloadBackgroundService)} start");

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

            // if (bool.TryParse(_configuration["MemoryCacheReset"], out bool isReset))
            // {
            //     _cacheProvider.Reset();
            // }
            _logger.LogInformation($"{nameof(ConfigReloadBackgroundService)} end");
            await Task.Delay(_interval, stoppingToken);
        }
    }
}