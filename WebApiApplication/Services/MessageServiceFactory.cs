using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Services
{
    public class MessageRequestDto
    {
        
    }
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(MessageRequestDto request);
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

        public IMessageService CreateService(ENUM_MESSAGE_TYPE type)
        {
            if (type == ENUM_MESSAGE_TYPE.SMS) return _serviceProvider.GetService<SMSMessageService>();
            else if (type == ENUM_MESSAGE_TYPE.EMAIL) return _serviceProvider.GetService<EmailMessageService>();
            else if (type == ENUM_MESSAGE_TYPE.KAKAO) return _serviceProvider.GetService<KakaoMessageService>();

            throw new Exception("not create service instance.");
        }
    }

    public enum ENUM_MESSAGE_TYPE
    {
        SMS = 1,
        EMAIL,
        KAKAO,
    }
    
    public class SMSMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            return false;
        }
    }
    
    public class EmailMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            return false;
        }
    }
    
    public class KakaoMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            return false;
        }
    }
}