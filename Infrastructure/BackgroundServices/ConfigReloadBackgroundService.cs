using Application.Script;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using eXtensionSharp;
using Infrastructure.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class ConfigReloadBackgroundService : BackgroundServiceBase
{
    /// <summary>
    /// interval (default : 30sec)
    /// </summary>
    private readonly int _interval = 1000 * 30;
    /// <summary>
    /// background service에서 IOC 컨테이너 의존석 주입이 자동으로 지원하지 않으므로 Scope로 생성하도록 아래와 같이 함.
    /// 만약 background service가 오직 asp.net core에서 호스팅 된다면 IServiceProvider또는 의존성 주입 객체를 바로 사용해도 되겠지만
    /// Console Application용도로는 IServiceScopeFactory를 사용해야 한다.
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static object _syncObj = new object();
    
    public ConfigReloadBackgroundService(ILogger<ConfigReloadBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        this._serviceScopeFactory = serviceScopeFactory;
    }

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
                    var config = scope.ServiceProvider.GetService<IOptionsMonitor<ScriptLoaderConfig>>();
                    var csScriptLoader = scope.ServiceProvider.GetService<SharpScriptLoader>();
                    var jsScriptLoader = scope.ServiceProvider.GetService<JsScriptLoader>();
                    
                    if (config.xIsNotEmpty())
                    {
                        var real = config.CurrentValue;
                        if (real.Reload.xIsTrue())
                        {
                            csScriptLoader.Reset();
                            jsScriptLoader.Reset();
                            config.CurrentValue.Reload = false;
                        }
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