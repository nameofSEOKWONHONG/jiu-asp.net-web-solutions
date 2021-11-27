using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Microsoft.Extensions.Configuration;

namespace ClientApplication.ViewModel
{
    public class ViewModelDependencyInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CounterStateViewModel>();
        }
    }
}