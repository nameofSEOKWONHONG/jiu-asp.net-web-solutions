using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Abstract;

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