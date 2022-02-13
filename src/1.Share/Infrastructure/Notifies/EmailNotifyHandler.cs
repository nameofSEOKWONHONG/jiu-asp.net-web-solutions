using Application.Infrastructure.Message;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class EmailNotifyHandler : INotificationHandler<NotifyMessage>
    {
        private readonly INotifyMessageProvider _notifyMessageProvider;
        public EmailNotifyHandler(MessageProviderResolver resolver)
        {
            _notifyMessageProvider = resolver(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL);
        }
        public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
        {
            if (!notification.MessageTypes.Contains(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL)) return;
            await _notifyMessageProvider.SendMessageAsync(notification.NotifyMessageRequest);
        }
    }
}