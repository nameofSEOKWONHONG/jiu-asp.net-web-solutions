using Application.Infrastructure.Message;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Renci.SshNet.Messages;

namespace Infrastructure.Abstract;

/// <summary>
/// backgroundservice 기본
/// </summary>
public abstract class BackgroundServiceBase : BackgroundService
{
    protected ILogger _logger;

    protected BackgroundServiceBase(ILogger logger)
    {
        this._logger = logger;
    }
}

/// <summary>
/// 메세지 전송 backgroundservice
/// </summary>
public abstract class MessageNotifyBackgroundServiceBase : BackgroundServiceBase
{
    protected MessageProviderResolver _messageProviderResolver;
    protected INotifyMessageProvider _notifyMessageProvider;
    
    protected MessageNotifyBackgroundServiceBase(ILogger logger, MessageProviderResolver messageProviderResolver) : base(logger)
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
    protected ParallelBackgroundServiceBase(ILogger logger, int maxDegreeOfParallelism = 1) : base(logger)
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