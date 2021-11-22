using System.Collections.Generic;
using System.Reflection;
using Application.Abstract;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecastApplication.Services.Abstract;
using WeatherForecastApplication.Services.Concrete;

namespace WeatherForecastApplication
{
    public class WeatherForecastApplicationInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IWeatherForcastService, WeatherForcastService>();
        }
    }
    
    public class WeatherForecastCQRSInjector : CQRSInjectorBase
    {
        public override Assembly GetAssembly()
        {
            return Assembly.Load("WeatherForecastApplication");
        }
    }

    public static class WeatherForecastApplicationInjectorExtension
    {
        public static void AddWeatherForecastApplicationInjector(this IServiceCollection services)
        {
            var injector = new WeatherForecastApplicationInjector();
            injector.Inject(services);
        }

        public static void AddWeatherForecastApplicationCQRSInjector(this IServiceCollection services)
        {
            var injector = new WeatherForecastCQRSInjector();
            services.AddMediatR(injector.GetAssembly());
        }
    } 
}