using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Domain.Enums;
using Application.Request;
using Application.Response;
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
                    switch (key)
                    {
                        case ENUM_NOTIFY_MESSAGE_TYPE.SMS : return provider.GetRequiredService<SmsNotifyMessageProvider>();
                        case ENUM_NOTIFY_MESSAGE_TYPE.EMAIL: return provider.GetRequiredService<EmailNotifyMessageProvider>();
                        case ENUM_NOTIFY_MESSAGE_TYPE.KAKAO : return provider.GetRequiredService<KakaoNotifyMessageProvider>();
                        default: throw new KeyNotFoundException();
                    }
                });
        }
    }

    public static class NotifyMessageProviderInjectorExtension
    {
        public static void AddMessageProviderInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new NotifyMessageProviderInjector()
            }, services, null);
            diCore.Inject();
        }
    }
}