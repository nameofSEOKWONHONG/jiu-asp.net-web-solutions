﻿using Application.Request;
using Domain.Enums;
using MediatR;

namespace Infrastructure.Notifies
{
    public class MessageNotify : INotification
    {
        public ENUM_MESSAGE_TYPE[] MessageTypes { get; set; }
        public IMessageRequest MessageRequest { get; set; }
    }
}