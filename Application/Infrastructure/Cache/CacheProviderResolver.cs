using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Application.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure.Cache
{
    public enum ENUM_CACHE_TYPE
    {
        MEMORY,
        REDIS,
    }
    
    public delegate ICacheProvider CacheProviderResolver(ENUM_CACHE_TYPE type);
    public class CacheProviderInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddTransient<MemoryCacheProvider>();
            services.AddTransient<RedisCacheProvider>();
            services.AddTransient<CacheProviderResolver>(provider => key =>
            {
                switch (key)
                {
                    case ENUM_CACHE_TYPE.MEMORY : return provider.GetService<MemoryCacheProvider>();
                    case ENUM_CACHE_TYPE.REDIS: return provider.GetService<RedisCacheProvider>();
                    default: throw new KeyNotFoundException();
                }
            });
        }
    }

    public static class CacheProviderInjectorExtension
    {
        public static void AddCacheProviderInject(this IServiceCollection services)
        {
            var injector = new CacheProviderInjector();
            injector.Inject(services);
        }
    }
 
}