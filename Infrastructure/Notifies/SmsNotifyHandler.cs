using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Application.Request;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class SmsNotifyHandler : INotificationHandler<MessageNotify>
    {
        private readonly IMessageProvider _messageProvider;
        public SmsNotifyHandler(MessageProviderResolver resolver)
        {
            _messageProvider = resolver(ENUM_MESSAGE_TYPE.SMS);
        }
        public async Task Handle(MessageNotify notification, CancellationToken cancellationToken)
        {
            if(!notification.MessageTypes.Contains(ENUM_MESSAGE_TYPE.SMS)) return;
            await _messageProvider.SendMessageAsync(new MessageRequestDto());
        }
    }
}