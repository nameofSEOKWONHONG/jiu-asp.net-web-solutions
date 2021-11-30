using System.Collections.Generic;
using System.Reflection;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using TodoApplication;
using WeatherForecastApplication;
using WebApiApplication.Services;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Extensions
{
    /// <summary>
    /// TODO : 서비스 프로젝트 분리, 인증 부분을 제외한 로직 부분은 모두 외부 프로젝트로 분리한다.
    /// TODO : MediatR(CQRS) 개념으로 분리할지는 고려해 보자.
    /// 왜 인젝터라는 걸 만들어서 써야 하나?
    /// 보통 닷넷에서 IoC컨테이너는 Autofac을 사용하겠지만 개인적인 사용 경험으로는 Autofac을 사용하여 Assembly로드로 한번에 로드하여
    /// 사용하는 방식으로 대부분 사용할 것이다. 물론 개별로 설정하여 사용할 수 있지만, 대부분의 프로젝트에서 그런 케이스를 보지 못했다.
    /// 따라서 내부 구현에서 transient, soope, singleton을 별도로 구현해서 사용한다.
    /// 이런 구현이 좋은 방향일까... 절대 아니라고 생각한다. IoC컨테이너 설계 철학에도 맞지 않는다.
    /// 따라서, 대부분의 프로젝트를 작은 단위로 유지하면서 아래와 같이 공통으로 처리하는 Injector를 갖는 것이 최선이라고 생각된다.
    /// 물론 Autofac에서도 동일하게 만들수 있지만, dotnet core 3 이상 부터는 IoC컨테이너를 자체적으로 지원하므로 괜히 외부의존성을
    /// 늘릴 필요는 없다고 생각된다.
    /// 일부 사람들은 너무 구식 방식이라고 하던데... 어떻게 해야 최신으로 하는 것인지는 의문이다.
    /// </summary>
    internal class ServiceInjectorImpl : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services, IConfiguration configuration)
        {
            #region [factory correct pattern]
            services.AddMessageProviderInject();
            services.AddCacheProviderInject(configuration);
            #endregion
        }
    }

    internal static class ConfigureServicesExtensions
    {
        public static void AddConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddServices(services, configuration);
            AddAssemblies(services, configuration);
        }

        internal static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            var injectors = new List<DependencyInjectorBase>()
            {
                new InfrastructureInjector(),
                new WeatherForecastApplicationInjector(),
                new ServiceInjectorImpl(),
                new TodoApplicationInjector()
            };
            
            injectors.xForEach(item =>
            {
                item.Inject(services, configuration);
            });
        }
        
        /// <summary>
        /// MediatR은 지미보가드가 만든 CQRS 패턴을 구현한 프레임워크이다.
        /// 중계자 패턴으로 불리우는데 Command and Query Responsibility Segregation의 약자로
        /// MVVM, IoC와 같이 관심사 분리 기능이다.
        /// DDD에서 문자 그대로 Search와 그 이외의 기능으로 분리하는 것으로 DB까지 확장해 보면 
        /// Command는 RDBMS, Query는 NOSql로 구성하는 방식까지 확장 할 수 있겠다. (만드는 사람 맘이지만...)
        /// <see cref="https://github.com/jbogard/MediatR"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        internal static void AddAssemblies(IServiceCollection services, IConfiguration configuration)
        {
            var injectors = new List<CQRSInjectorBase>()
            {
                new InfrastructureCQRSInjector(),
                new WeatherForecastCQRSInjector(),
                new TodoApplicationCQRSInjector(),
            };

            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());
            
            injectors.xForEach(item =>
            {
                assemblies.Add(item.GetAssembly());
            });

            services.AddMediatR(assemblies.ToArray());
        }
    }
}