using ClientApplication.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace ClientApplication.ViewModel
{
    public class ViewModelInjector : InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddSingleton<CounterStateViewModel>();
        }
    }
}