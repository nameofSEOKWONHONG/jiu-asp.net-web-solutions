using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    public abstract class DependencyInjectorBase : IDependencyInjectorBase
    {
        public abstract void Inject(IServiceCollection services);
    }
}