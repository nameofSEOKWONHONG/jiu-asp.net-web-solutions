using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;

namespace ClientApplication.ViewModel
{
    public class ViewModelDependencyInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddSingleton<CounterStateViewModel>();
        }
    }
}