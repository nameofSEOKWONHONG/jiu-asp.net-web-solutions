using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    public interface IDependencyInjectorBase
    {
        void Inject(IServiceCollection services);
    }
}
