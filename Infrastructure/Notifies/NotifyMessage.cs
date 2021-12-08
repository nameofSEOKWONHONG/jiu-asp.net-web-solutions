using Application.Request;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class NotifyMessage : INotification
    {
        public ENUM_NOTIFY_MESSAGE_TYPE[] MessageTypes { get; set; }
        public INotifyMessageRequest NotifyMessageRequest { get; set; }
    }
}