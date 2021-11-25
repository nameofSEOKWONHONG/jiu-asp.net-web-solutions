using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Application.Request;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class KakaoNotifyHandler : INotificationHandler<MessageNotify>
    {
        private readonly IMessageProvider _messageProvider;
        public KakaoNotifyHandler(MessageProviderResolver resolver)
        {
            _messageProvider = resolver(ENUM_MESSAGE_TYPE.KAKAO);
        }
        public async Task Handle(MessageNotify notification, CancellationToken cancellationToken)
        {
            if(!notification.MessageTypes.Contains(ENUM_MESSAGE_TYPE.KAKAO)) return;
            await _messageProvider.SendMessageAsync(notification.MessageRequest);
        }
    }
}

