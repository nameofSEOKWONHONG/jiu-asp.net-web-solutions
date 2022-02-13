using Application.Infrastructure.Message;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class SmsNotifyHandler : INotificationHandler<NotifyMessage>
    {
        private readonly INotifyMessageProvider _notifyMessageProvider;
        public SmsNotifyHandler(MessageProviderResolver resolver)
        {
            _notifyMessageProvider = resolver(ENUM_NOTIFY_MESSAGE_TYPE.SMS);
        }
        public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
        {
            if(!notification.MessageTypes.Contains(ENUM_NOTIFY_MESSAGE_TYPE.SMS)) return;
            await _notifyMessageProvider.SendMessageAsync(notification.NotifyMessageRequest);
        }
    }
}