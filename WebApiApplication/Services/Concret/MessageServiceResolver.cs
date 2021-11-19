﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Application.Enums;
using Application.Request;
using Application.Response;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    /// <summary>
    /// Factory correct pattern
    /// </summary>
    public delegate IMessageService MessageServiceResolver(ENUM_MESSAGE_TYPE type);
    
    public class MessageServiceInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
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

    public static class MessageServiceInjectorExtension
    {
        public static void AddMessageServiceInject(this IServiceCollection services)
        {
            var injector = new MessageServiceInjector();
            injector.Inject(services);
        }
    }

    public class SMSMessageService : IMessageService
    {
        public async Task<IResult> SendMessageAsync(MessageRequestDto request)
        {
            
            await Task.Delay(1);
            return Result.Success(nameof(SMSMessageService));
        }
    }
    
    public class EmailMessageService : IMessageService
    {
        public async Task<IResult> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return Result.Success(nameof(EmailMessageService));
        }
    }
    
    public class KakaoMessageService : IMessageService
    {
        public async Task<IResult> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return Result.Success(nameof(EmailMessageService));
        }
    }
}