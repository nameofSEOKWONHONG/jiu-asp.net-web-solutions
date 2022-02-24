using System;
using System.Threading.Tasks;
using Application.Request;
using Domain.Response;

namespace Application.Infrastructure.Message
{
    public class SmsNotifyMessageProvider : NotifyMessageProviderBase
    {
        public override Task<IResultBase> SendMessageAsync(INotifyMessageRequest request)
        {
            throw new NotImplementedException();
        }

        public override object ConvertRequest(INotifyMessageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}