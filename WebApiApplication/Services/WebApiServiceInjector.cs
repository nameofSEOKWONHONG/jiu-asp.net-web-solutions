using System;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Abstract;
using SharedLibrary.Infrastructure;
using WebApiApplication.Infrastructure;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    public class WebApiServiceInjector : DependencyInjectorBase
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
            #endregion
            
            #region [factory anti pattern]
            services.AddTransient<BoatCreatorService>();
            services.AddTransient<CarCreatorService>();
            services.AddTransient<BusCreatorService>();
            services.AddSingleton<VehicleCreatorServiceFactory>();
            #endregion
            
            services.AddSingleton<IGenerateViewService, GenerateViewService>();
            
            services.AddScoped<ISessionContextService, SessionContextService>();

            services.AddCacheProviderInject();
        }
    }

    public static class WebApiServiceInjectorExtensions
    {
        public static void AddWebApiService(this IServiceCollection services)
        {
            var injector = new WebApiServiceInjector();
            injector.Inject(services);
        }
    }
}