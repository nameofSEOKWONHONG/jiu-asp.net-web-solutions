using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Abstract;

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

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="interval">second</param>
    protected BackgroundServiceBase(ILogger logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, int interval = 60)
    {
        this._logger = logger;
        this._configuration = configuration;
        this._serviceScopeFactory = serviceScopeFactory;
        this._interval = 1000 * interval;
    }

    protected abstract Task ExecuteCore(CancellationToken stopingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            this._logger.LogInformation($"start");
            await this.ExecuteCore(stoppingToken);
            this._logger.LogInformation($"end");
            await Task.Delay(_interval);
        }
    }
}

/// <summary>
/// 메세지 전송 backgroundservice
/// </summary>
public abstract class MessageNotifyBackgroundServiceBase : BackgroundServiceBase
{
    protected MessageProviderResolver _messageProviderResolver;
    protected INotifyMessageProvider _notifyMessageProvider;
    
    protected MessageNotifyBackgroundServiceBase(ILogger logger, 
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        MessageProviderResolver messageProviderResolver) : base(logger, configuration, serviceScopeFactory)
    {
        this._messageProviderResolver = messageProviderResolver;
        //default
        this._notifyMessageProvider = this._messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL);
    }
}

/// <summary>
/// 병렬 실행 backgroundservice
/// </summary>
public abstract class ParallelBackgroundServiceBase : BackgroundServiceBase
{
    protected readonly ParallelOptions _parallelOptions;
    protected ParallelBackgroundServiceBase(ILogger logger, 
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory, 
        int maxDegreeOfParallelism = 1) : base(logger, configuration, serviceScopeFactory)
    {
        this._parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
    }

    protected void ExecuteParallel()
    {
        
    }
}