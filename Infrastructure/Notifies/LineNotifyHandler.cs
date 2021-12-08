using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class LineNotifyHandler : INotificationHandler<NotifyMessage>
    {
        private readonly INotifyMessageProvider _notifyMessageProvider;
        public LineNotifyHandler(MessageProviderResolver resolver)
        {
            _notifyMessageProvider = resolver(ENUM_NOTIFY_MESSAGE_TYPE.LINE);
        }
        
        public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
        {
            if(!notification.MessageTypes.Contains(ENUM_NOTIFY_MESSAGE_TYPE.LINE)) return;
            await _notifyMessageProvider.SendMessageAsync(notification.NotifyMessageRequest);
        }
    }
}