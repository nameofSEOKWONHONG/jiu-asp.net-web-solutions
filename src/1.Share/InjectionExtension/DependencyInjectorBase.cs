using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstract
{
    /// <summary>
    /// service dependancy injection interface
    /// </summary>
    public interface IServiceInjectionBase
    {
        void Inject(IServiceCollection services, IConfiguration configuration);
    }

    /// <summary>
    /// 서비스 로더, 매뉴얼 서비스 등록을 지원함.
    /// </summary>
    public sealed class ServiceLoader
    {
        private readonly IServiceInjectionBase[] _dependencyInjectorBases;
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        public ServiceLoader(IServiceInjectionBase[] dependencyInjectorBases,
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