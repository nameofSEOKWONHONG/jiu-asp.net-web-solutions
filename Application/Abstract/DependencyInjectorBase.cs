using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    public abstract class DependencyInjectorBase
    {
        public abstract void Inject(IServiceCollection services, IConfiguration configuration);
    }

    public abstract class CQRSInjectorBase
    {
        public abstract Assembly GetAssembly();
    }
}