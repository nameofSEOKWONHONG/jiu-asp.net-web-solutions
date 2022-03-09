using Application.Infrastructure.Message;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Abstract;

/// <summary>
/// 메세지 전송 backgroundservice, BackgroundServiceBase기반으로 MessageProviderResolver/INotifyMessageProvider를 기본으로 제공한다.
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