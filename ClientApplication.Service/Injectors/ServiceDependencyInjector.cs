using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Abstract;

namespace ClientApplication.Service
{
    public sealed class ServiceDependencyInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<LoginCheckService>();
            services.AddScoped<WeatherForecastService>();
        }
    }
}