using System.Reflection;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;

namespace InjectionExtension;

public static class ServiceLifeTimeBuilder
{
    private static Dictionary<ENUM_LIFE_TIME_TYPE, Action<IServiceCollection, AddServiceAttribute, Type>>
        _serviceLifeTimeStates =
            new()
            {
                {
                    ENUM_LIFE_TIME_TYPE.Scope, (s, attr, impl) =>
                    {
                        attr.InterfaceType.xIfNotEmpty(() =>
                        {
                            s.AddScoped(attr.InterfaceType, impl);
                        }, () =>
                        {
                            s.AddScoped(impl);
                        });
                    }
                },
                {
                    ENUM_LIFE_TIME_TYPE.Transient, (s, attr, impl) =>
                    {
                        attr.InterfaceType.xIfNotEmpty(() =>
                        {
                            s.AddTransient(attr.InterfaceType, impl);
                        }, () =>
                        {
                            s.AddTransient(impl);
                        });
                    }
                },
                {
                    ENUM_LIFE_TIME_TYPE.Singleton, (s, attr, impl) =>
                    {
                        attr.InterfaceType.xIfNotEmpty(() =>
                        {
                            s.AddSingleton(attr.InterfaceType, impl);
                        }, () =>
                        {
                            s.AddSingleton(impl);
                        });
                    }
                }
            };


    /// <summary>
    /// ioc container에 의존성 주입 작업을 수행합니다.
    /// ServiceLifeTimeAttribute를 사용한 클래스를 자동으로 의존성 주입을 수행합니다.
    /// </summary>
    /// <param name="services"></param>
    public static void AddLifeTimeBuild(this IServiceCollection services)
    {
        var serviceLifeTimeTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Select(x => Assembly.Load(x.FullName))
            .SelectMany(x => x.GetExportedTypes())
            .ToList()
            .Where(x => x.GetCustomAttribute<AddServiceAttribute>(false) != null);
        
        serviceLifeTimeTypes.xForEach(type =>
        {
            var attr = type.GetCustomAttribute<AddServiceAttribute>(false);
            if (attr is AddServiceAttribute serviceLifeTimeAttribute)
            {
                _serviceLifeTimeStates[serviceLifeTimeAttribute.LifeTimeType](services, serviceLifeTimeAttribute, type);
            }
        });
    }
}