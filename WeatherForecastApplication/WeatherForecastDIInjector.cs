using System.Reflection;
using Application.Abstract;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecastApplication.Services.Abstract;
using WeatherForecastApplication.Services.Concrete;

namespace WeatherForecastApplication
{
    public class WeatherForecastDIInjector : DependencyInjectorBase
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
}