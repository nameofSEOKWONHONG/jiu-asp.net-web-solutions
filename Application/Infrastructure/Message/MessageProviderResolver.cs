using System;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Application.Infrastructure.Message
{
    /// <summary>
    /// Factory correct pattern
    /// </summary>
    public delegate INotifyMessageProvider MessageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE type);
    
    internal class NotifyMessageProviderInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SmsNotifyMessageProvider>()
                .AddSingleton<EmailNotifyMessageProvider>()
                .AddSingleton<KakaoNotifyMessageProvider>()
                .AddSingleton<MessageProviderResolver>(provider => key =>
                {
                    if(key == ENUM_NOTIFY_MESSAGE_TYPE.SMS) return provider.GetRequiredService<SmsNotifyMessageProvider>();
                    else if(key == ENUM_NOTIFY_MESSAGE_TYPE.EMAIL) return provider.GetRequiredService<EmailNotifyMessageProvider>();
                    else if (key == ENUM_NOTIFY_MESSAGE_TYPE.KAKAO)
                        return provider.GetRequiredService<KakaoNotifyMessageProvider>();
                    else throw new NotImplementedException();
                });
        }
    }

    public static class NotifyMessageProviderInjectorExtension
    {
        public static void AddMessageProviderInjector(this IServiceCollection services)
        {
            var impl = new DependencyInjectorImpl(new[]
            {
                new NotifyMessageProviderInjector()
            }, services, null);
            impl.Inject();
        }
    }
}