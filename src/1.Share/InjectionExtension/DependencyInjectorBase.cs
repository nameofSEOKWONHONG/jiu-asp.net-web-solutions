using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    /// <summary>
    /// di 공통화 interface
    /// </summary>
    public interface IDependencyInjectorBase
    {
        void Inject(IServiceCollection services, IConfiguration configuration);
    }

    /// <summary>
    /// DI Loader 
    /// </summary>
    public sealed class DependencyInjectionLoader
    {
        private readonly IDependencyInjectorBase[] _dependencyInjectorBases;
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        public DependencyInjectionLoader(IDependencyInjectorBase[] dependencyInjectorBases,
            IServiceCollection services,
            IConfiguration configuration)
        {
            _dependencyInjectorBases = dependencyInjectorBases;
            _services = services;
            _configuration = configuration;
        }

        public void Inject()
        {
            _dependencyInjectorBases.xForEach(item =>
            {
                item.Inject(_services, _configuration);
            });
        }
    }
}