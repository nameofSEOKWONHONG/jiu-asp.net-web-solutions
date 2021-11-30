using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Application.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure.Cache
{
    public enum ENUM_CACHE_TYPE
    {
        MEMORY,
        REDIS,
        LITEDB,
    }
    
    public delegate ICacheProvider CacheProviderResolver(ENUM_CACHE_TYPE type);
    internal class CacheProviderInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisConfiguration:Uri"];
            });
            services.AddSingleton<MemoryCacheProvider>();
            services.AddSingleton<DistributeCacheProvider>();
            services.AddSingleton<LiteDbCacheProvider>();
            services.AddTransient<CacheProviderResolver>(provider => key =>
            {
                switch (key)
                {
                    case ENUM_CACHE_TYPE.MEMORY : return provider.GetService<MemoryCacheProvider>();
                    case ENUM_CACHE_TYPE.REDIS: return provider.GetService<DistributeCacheProvider>();
                    case ENUM_CACHE_TYPE.LITEDB: return provider.GetService<LiteDbCacheProvider>();
                    default: throw new KeyNotFoundException();
                }
            });
        }
    }

    public static class CacheProviderInjectorExtension
    {
        public static void AddCacheProviderInject(this IServiceCollection services, IConfiguration configuration)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new CacheProviderInjector()
            }, services, configuration);
            diCore.Inject();
        }
    }
 
}