using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Domain.Enums;
using Application.Request;
using Application.Response;

namespace Application.Infrastructure.Message
{
    /// <summary>
    /// Factory correct pattern
    /// </summary>
    public delegate IMessageProvider MessageProviderResolver(ENUM_MESSAGE_TYPE type);
    
    public class MessageProviderInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddTransient<SmsMessageProvider>();
            services.AddTransient<EmailMessageProvider>();
            services.AddTransient<KakaoMessageProvider>();
            services.AddTransient<MessageProviderResolver>(provider => key =>
            {
                switch (key)
                {
                    case ENUM_MESSAGE_TYPE.SMS : return provider.GetService<SmsMessageProvider>();
                    case ENUM_MESSAGE_TYPE.EMAIL: return provider.GetService<EmailMessageProvider>();
                    case ENUM_MESSAGE_TYPE.KAKAO : return provider.GetService<KakaoMessageProvider>();
                    default: throw new KeyNotFoundException();
                }
            });
        }
    }

    public static class MessageProviderInjectorExtension
    {
        public static void AddMessageProviderInject(this IServiceCollection services)
        {
            var injector = new MessageProviderInjector();
            injector.Inject(services);
        }
    }


    


}