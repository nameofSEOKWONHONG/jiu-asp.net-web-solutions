using System.Collections.Generic;
using System.Reflection;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    public interface IDependencyInjectorBase
    {
        void Inject(IServiceCollection services, IConfiguration configuration);
    }

    public sealed class DependencyInjector
    {
        private readonly IDependencyInjectorBase[] _dependencyInjectorBases;
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        public DependencyInjector(IDependencyInjectorBase[] dependencyInjectorBases,
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