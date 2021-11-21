using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    public abstract class DependencyInjectorBase : IDependencyInjectorBase
    {
        public abstract void Inject(IServiceCollection services);
    }

    public abstract class CQRSInjectorBase
    {
        public abstract Assembly GetAssembly();
    }
}