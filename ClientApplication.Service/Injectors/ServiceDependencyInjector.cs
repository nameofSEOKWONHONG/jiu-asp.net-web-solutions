using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Microsoft.Extensions.Configuration;

namespace ClientApplication.Service
{
    public sealed class ServiceDependencyInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<LoginCheckService>();
            services.AddScoped<WeatherForecastService>();
        }
    }
}