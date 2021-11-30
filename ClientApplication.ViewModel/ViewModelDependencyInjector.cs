using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Microsoft.Extensions.Configuration;

namespace ClientApplication.ViewModel
{
    public class ViewModelDependencyInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CounterStateViewModel>();
        }
    }
}