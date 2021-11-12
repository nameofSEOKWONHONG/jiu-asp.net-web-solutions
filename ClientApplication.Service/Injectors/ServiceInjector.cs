using ClientApplication.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace ClientApplication.Service
{
    public sealed class ServiceInjector : InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<LoginCheckService>();
            services.AddScoped<WeatherForecastService>();
        }
    }
}