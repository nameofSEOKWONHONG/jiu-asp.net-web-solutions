using Microsoft.Extensions.DependencyInjection;

namespace ClientApplication.Abstract
{
    public interface IInjectorBase
    {
        void Inject(IServiceCollection services);
    }
}
