using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Enums;
using SharedLibrary.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    /// <summary>
    /// Factory correct pattern
    /// </summary>
    public delegate IMessageService MessageServiceResolver(ENUM_MESSAGE_TYPE type);

    public static class MessageServiceInjector
    {
        public static void AddMessageServiceInject(this IServiceCollection services)
        {
            services.AddTransient<SMSMessageService>();
            services.AddTransient<EmailMessageService>();
            services.AddTransient<KakaoMessageService>();
            services.AddTransient<MessageServiceResolver>(provider => key =>
            {
                switch (key)
                {
                    case ENUM_MESSAGE_TYPE.SMS : return provider.GetService<SMSMessageService>();
                    case ENUM_MESSAGE_TYPE.EMAIL: return provider.GetService<EmailMessageService>();
                    case ENUM_MESSAGE_TYPE.KAKAO : return provider.GetService<KakaoMessageService>();
                    default: throw new KeyNotFoundException();
                }
            });
        }
    }
    // public class MessageServiceFactory
    // {   
    //     private readonly IServiceProvider _serviceProvider;
    //     public MessageServiceFactory(MessageServiceResolver serviceProvider)
    //     {
    //         _serviceProvider = serviceProvider;
    //     }
    //
    //     public IMessageService CreateService<T>() where T : IMessageService
    //     {
    //         return _serviceProvider.GetService<T>();
    //     }
    //
    //     public IMessageService CreateService(ENUM_MESSAGE_TYPE type)
    //     {
    //         if (type == ENUM_MESSAGE_TYPE.SMS) return _serviceProvider.GetService<SMSMessageService>();
    //         else if (type == ENUM_MESSAGE_TYPE.EMAIL) return _serviceProvider.GetService<EmailMessageService>();
    //         else if (type == ENUM_MESSAGE_TYPE.KAKAO) return _serviceProvider.GetService<KakaoMessageService>();
    //
    //         throw new Exception("not create service instance.");
    //     }
    // }


    
    public class SMSMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return false;
        }
    }
    
    public class EmailMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return false;
        }
    }
    
    public class KakaoMessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return false;
        }
    }
}