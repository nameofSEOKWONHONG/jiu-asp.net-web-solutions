using System;
using System.Collections.Generic;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Application.Infrastructure;
using WebApiApplication.Infrastructure;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    /// <summary>
    /// TODO : 서비스 프로젝트 분리, 인증 부분을 제외한 로직 부분은 모두 외부 프로젝트로 분리한다.
    /// TODO : MediatR(CQRS) 개념으로 분리할지는 고려해 보자.
    /// </summary>
    public class ServerServiceInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IWeatherForcastService, WeatherForcastService>();
            
            services.AddSingleton<DbL4Provider>();
            services.AddSingleton<DbL4Interceptor>();        
            
            #region [factory correct pattern]
            services.AddMessageServiceInject();
            services.AddCacheProviderInject();
            #endregion
            
            #region [factory anti pattern]
            services.AddTransient<BoatCreatorService>();
            services.AddTransient<CarCreatorService>();
            services.AddTransient<BusCreatorService>();
            services.AddSingleton<VehicleCreatorServiceFactory>();
            #endregion
            
            services.AddSingleton<IGenerateViewService, GenerateViewService>();
            
            services.AddScoped<ISessionContextService, SessionContextService>();
        }
    }

    public static class ServiceInjectorExtensions
    {
        public static void AddRegisterService(this IServiceCollection services)
        {
            var injectors = new List<DependencyInjectorBase>()
            {
                new ServerServiceInjector()
            };
            injectors.xForEach(item =>
            {
                item.Inject(services);
            });
        }
    }
}