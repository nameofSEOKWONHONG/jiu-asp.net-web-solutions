﻿using System;
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
        LITEDB,
    }
    
    public delegate ICacheProvider CacheProviderResolver(ENUM_CACHE_TYPE type);
    public class CacheProviderInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheProvider>();
            services.AddSingleton<RedisCacheProvider>();
            services.AddSingleton<LiteDbCacheProvider>();
            services.AddTransient<CacheProviderResolver>(provider => key =>
            {
                switch (key)
                {
                    case ENUM_CACHE_TYPE.MEMORY : return provider.GetService<MemoryCacheProvider>();
                    case ENUM_CACHE_TYPE.REDIS: return provider.GetService<RedisCacheProvider>();
                    case ENUM_CACHE_TYPE.LITEDB: return provider.GetService<LiteDbCacheProvider>();
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