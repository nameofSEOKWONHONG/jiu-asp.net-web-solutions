using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Application.Request;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class EMailNotifyHandler : INotificationHandler<MessageNotify>
    {
        private readonly IMessageProvider _messageProvider;
        public EMailNotifyHandler(MessageProviderResolver resolver)
        {
            _messageProvider = resolver(ENUM_MESSAGE_TYPE.EMAIL);
        }
        public async Task Handle(MessageNotify notification, CancellationToken cancellationToken)
        {
            if (!notification.MessageTypes.Contains(ENUM_MESSAGE_TYPE.EMAIL)) return;
            await _messageProvider.SendMessageAsync(new MessageRequestDto());
        }
    }
}