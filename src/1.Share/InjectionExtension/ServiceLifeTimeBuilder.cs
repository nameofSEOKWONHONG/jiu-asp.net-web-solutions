using System.Reflection;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;

namespace InjectionExtension;

public static class ServiceLifeTimeBuilder
{
    private static Dictionary<ENUM_LIFE_TYPE, Action<IServiceCollection, ServiceLifeTimeAttribute, Type>>
        _serviceLifeTimeStates =
            new()
            {
                {
                    ENUM_LIFE_TYPE.Scope, (s, attr, impl) =>
                    {
                        attr.TypeOfInterface.xIfNotEmpty(() =>
                        {
                            s.AddScoped(attr.TypeOfInterface, impl);
                        }, () =>
                        {
                            s.AddScoped(impl);
                        });
                    }
                },
                {
                    ENUM_LIFE_TYPE.Transient, (s, attr, impl) =>
                    {
                        attr.TypeOfInterface.xIfNotEmpty(() =>
                        {
                            s.AddTransient(attr.TypeOfInterface, impl);
                        }, () =>
                        {
                            s.AddTransient(impl);
                        });
                    }
                },
                {
                    ENUM_LIFE_TYPE.Singleton, (s, attr, impl) =>
                    {
                        attr.TypeOfInterface.xIfNotEmpty(() =>
                        {
                            s.AddSingleton(attr.TypeOfInterface, impl);
                        }, () =>
                        {
                            s.AddSingleton(impl);
                        });
                    }
                }
            };

    // private static Dictionary<ENUM_LIFE_TYPE, Action<IServiceCollection, ServiceLifeTimeFactoryAttribute>> _serviceLifeTimeFactoryStates
    //     = new Dictionary<ENUM_LIFE_TYPE, Action<IServiceCollection, ServiceLifeTimeFactoryAttribute>>()
    //     {
    //         {
    //             ENUM_LIFE_TYPE.Scope, (s, attr) =>
    //             {
    //                 s.AddScoped(attr.Resolver, implementationFactory =>
    //                 {
    //                     implementationFactory.GetRequiredService(attr.ImplementType);
    //                 })
    //                 var @delegate = Delegate.CreateDelegate(attr.TypeOfInterface, attr.MethodInfo);
    //                 
    //                 attr.ImplementType.dele
    //                 s.AddSingleton(provider => key => { })
    //                 s.AddScoped(attr.ResolverType, factory => factory.GetRequiredService(attr.ImplementType));
    //             }
    //         },
    //         {
    //             ENUM_LIFE_TYPE.Transient, (s, attr) =>
    //             {
    //                 s.AddTransient(attr.ResolverType, factory => factory.GetRequiredService(attr.ImplementType));
    //             }
    //         },
    //         {
    //             ENUM_LIFE_TYPE.Singleton, (s, attr) =>
    //             {
    //                 s.AddSingleton(attr.ResolverType, factory => factory.GetRequiredService(attr.ImplementType));
    //             }
    //         }
    //     };


    /// <summary>
    /// ioc container에 의존성 주입 작업을 수행합니다.
    /// ServiceLifeTimeAttribute를 사용한 클래스를 자동으로 의존성 주입을 수행합니다.
    /// </summary>
    /// <param name="services"></param>
    public static void AddLifeTime(this IServiceCollection services)
    {
        var serviceLifeTimeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => Assembly.Load(x.FullName))
                .SelectMany(x => x.GetExportedTypes())
                .ToList()
                .Where(x => x.GetCustomAttribute(typeof(ServiceLifeTimeAttribute)) != null);
        
        serviceLifeTimeTypes.xForEach(type =>
        {
            var attr = type.GetCustomAttribute(typeof(ServiceLifeTimeAttribute), true);
            if (attr is ServiceLifeTimeAttribute serviceLifeTimeAttribute)
            {
                _serviceLifeTimeStates[serviceLifeTimeAttribute.LifeType](services, serviceLifeTimeAttribute, type);
            }
        });
        
        // var serviceLifeTimeFactoryTypes = AppDomain.CurrentDomain.GetAssemblies()
        //     .Select(x => Assembly.Load(x.FullName))
        //     .SelectMany(x => x.GetExportedTypes())
        //     .ToList()
        //     .Where(x => x.GetCustomAttribute(typeof(ServiceLifeTimeFactoryAttribute)) != null);
        //
        // serviceLifeTimeFactoryTypes.xForEach(type =>
        // {
        //     var attr = type.GetCustomAttribute(typeof(ServiceLifeTimeFactoryAttribute), true);
        //     if (attr is ServiceLifeTimeFactoryAttribute serviceLifeTimeFactoryAttribute)
        //     {
        //         _serviceLifeTimeFactoryStates[serviceLifeTimeFactoryAttribute.LifeType](services, serviceLifeTimeFactoryAttribute);
        //     }
        // });
    }
}