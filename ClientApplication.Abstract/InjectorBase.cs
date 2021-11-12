using Microsoft.Extensions.DependencyInjection;

namespace ClientApplication.Abstract
{
    public abstract class InjectorBase : IInjectorBase
    {
        public abstract void Inject(IServiceCollection services);
    }
}