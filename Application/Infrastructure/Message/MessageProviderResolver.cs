using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;

namespace Application.Infrastructure.Message
{
    /// <summary>
    /// Factory correct pattern
    /// </summary>
    public delegate INotifyMessageProvider MessageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE type);
    
    internal class NotifyMessageProviderInjector : IDependencyInjectorBase
    {
        private readonly Dictionary<ENUM_NOTIFY_MESSAGE_TYPE, Func<IServiceProvider, INotifyMessageProvider>>
            _notifyState =
                new Dictionary<ENUM_NOTIFY_MESSAGE_TYPE, Func<IServiceProvider, INotifyMessageProvider>>()
                {
                    { ENUM_NOTIFY_MESSAGE_TYPE.SMS, (s) => s.GetRequiredService<SmsNotifyMessageProvider>() },
                    { ENUM_NOTIFY_MESSAGE_TYPE.EMAIL, (s) => s.GetRequiredService<EmailNotifyMessageProvider>() },
                    { ENUM_NOTIFY_MESSAGE_TYPE.KAKAO, (s) => s.GetRequiredService<KakaoNotifyMessageProvider>() },
                };
            
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SmsNotifyMessageProvider>()
                .AddSingleton<EmailNotifyMessageProvider>()
                .AddSingleton<KakaoNotifyMessageProvider>()
                .AddSingleton<MessageProviderResolver>(provider => key =>
                {
                    var func = _notifyState[key];
                    if (func.xIsEmpty()) throw new NotImplementedException($"key {key.ToString()} not implemented");
                    return func(provider);
                });
        }
    }

    public static class NotifyMessageProviderInjectorExtension
    {
        public static void AddMessageProviderInjector(this IServiceCollection services)
        {
            var impl = new DependencyInjector(new[]
            {
                new NotifyMessageProviderInjector()
            }, services, null);
            impl.Inject();
        }
    }
}