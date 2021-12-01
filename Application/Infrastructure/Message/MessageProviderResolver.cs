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
    public delegate IMessageProvider MessageProviderResolver(ENUM_MESSAGE_TYPE type);
    
    internal class MessageProviderInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SmsMessageProvider>()
                .AddSingleton<EmailMessageProvider>()
                .AddSingleton<KakaoMessageProvider>()
                .AddSingleton<MessageProviderResolver>(provider => key =>
                {
                    switch (key)
                    {
                        case ENUM_MESSAGE_TYPE.SMS : return provider.GetRequiredService<SmsMessageProvider>();
                        case ENUM_MESSAGE_TYPE.EMAIL: return provider.GetRequiredService<EmailMessageProvider>();
                        case ENUM_MESSAGE_TYPE.KAKAO : return provider.GetRequiredService<KakaoMessageProvider>();
                        default: throw new KeyNotFoundException();
                    }
                });
        }
    }

    public static class MessageProviderInjectorExtension
    {
        public static void AddMessageProviderInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new MessageProviderInjector()
            }, services, null);
            diCore.Inject();
        }
    }
}