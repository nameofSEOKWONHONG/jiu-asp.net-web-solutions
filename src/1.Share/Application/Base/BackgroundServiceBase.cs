using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Base;

/// <summary>
/// backgroundservice 기본
/// </summary>
public abstract class BackgroundServiceBase : BackgroundService
{
    protected ILogger _logger;
    protected IConfiguration _configuration;
    /// <summary>
    /// background service에서 IOC 컨테이너 의존석 주입이 자동으로 지원하지 않으므로 Scope로 생성하도록 아래와 같이 함.
    /// 만약 background service가 오직 asp.net core에서 호스팅 된다면 IServiceProvider또는 의존성 주입 객체를 바로 사용해도 되겠지만
    /// Console Application용도로는 IServiceScopeFactory를 사용해야 한다.
    /// </summary>
    protected IServiceScopeFactory _serviceScopeFactory;
    protected int _interval = 1000;

    protected readonly TimeSpan _from = TimeSpan.Zero;
    protected readonly TimeSpan _to = TimeSpan.Zero;
    private readonly string _dateFormat = "yyyyMMddHHmmss";
    private readonly bool _useTimeZone = false;
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="interval">second</param>
    /// <param name="timeFrom">from time (000000)</param>
    /// <param name="timeTo">to time (060000)</param>
    protected BackgroundServiceBase(ILogger logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, 
        int interval = 60, string timeFrom = "", string timeTo = "")
    {
        this._logger = logger;
        this._configuration = configuration;
        this._serviceScopeFactory = serviceScopeFactory;
        this._interval = 1000 * interval;
        _useTimeZone = timeFrom.xIsNotEmpty() && timeTo.xIsNotEmpty();
        if (_useTimeZone)
        {
            this._from = DateTime
                .ParseExact($"{DateTime.Now.ToString("YYMMdd")}{timeFrom}", _dateFormat, CultureInfo.InvariantCulture)
                .TimeOfDay;
            this._to = DateTime
                .ParseExact($"{DateTime.Now.ToString("YYMMdd")}{timeTo}", _dateFormat, CultureInfo.InvariantCulture)
                .TimeOfDay;            
        }
    }

    /// <summary>
    /// single 구조로 실행할 경우
    /// </summary>
    /// <param name="stopingToken"></param>
    /// <returns></returns>
    protected abstract Task OnRunAsync(CancellationToken stopingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            this._logger.LogInformation($"service start");
            try
            {
                if (_useTimeZone)
                {
                    var nowTime = DateTime.Now.TimeOfDay;
                    if (nowTime.xBetween(_from, _to))
                    {
                        await this.OnRunAsync(stoppingToken);
                    }
                    else
                    {
                        this._logger.LogTrace($"{_from.ToString()} {_to.ToString()} not execution time");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            this._logger.LogInformation($"service end");
            await Task.Delay(_interval, stoppingToken);
        }
    }
}



