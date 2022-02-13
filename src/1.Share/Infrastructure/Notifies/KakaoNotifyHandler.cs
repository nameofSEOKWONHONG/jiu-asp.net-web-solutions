using Application.Infrastructure.Message;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class KakaoNotifyHandler : INotificationHandler<NotifyMessage>
    {
        private readonly INotifyMessageProvider _notifyMessageProvider;
        public KakaoNotifyHandler(MessageProviderResolver resolver)
        {
            _notifyMessageProvider = resolver(ENUM_NOTIFY_MESSAGE_TYPE.KAKAO);
        }
        public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
        {
            if(!notification.MessageTypes.Contains(ENUM_NOTIFY_MESSAGE_TYPE.KAKAO)) return;
            await _notifyMessageProvider.SendMessageAsync(notification.NotifyMessageRequest);
        }
    }
}

