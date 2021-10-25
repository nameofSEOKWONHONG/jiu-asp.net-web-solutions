using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Services
{
    public interface IMessageService
    {
        public string SendMessage();
    }

    public class MessageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMessageService CreateService<T>() where T : IMessageService
        {
            return _serviceProvider.GetService<T>();
        }
    }
    
    public class SMSMessageService : IMessageService
    {
        public string SendMessage()
        {
            return "send sms";
        }
    }
    
    public class EmailMessageService : IMessageService
    {
        public string SendMessage()
        {
            return "send email";
        }
    }
    
    public class KakaoMessageService : IMessageService
    {
        public string SendMessage()
        {
            return "send kakao";
        }
    }
}