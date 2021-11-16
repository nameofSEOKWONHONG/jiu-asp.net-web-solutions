using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.Abstract
{
    public interface IDependencyInjectorBase
    {
        void Inject(IServiceCollection services);
    }
}
