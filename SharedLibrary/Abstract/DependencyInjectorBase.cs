using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.Abstract
{
    public abstract class DependencyInjectorBase : IDependencyInjectorBase
    {
        public abstract void Inject(IServiceCollection services);
    }
}