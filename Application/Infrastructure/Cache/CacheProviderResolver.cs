using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Application.Abstract;
using Application.Infrastructure.Cache.MSFaster;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure.Cache
{
    public enum ENUM_CACHE_TYPE
    {
        MEMORY,
        REDIS,
        LITEDB,
        FASTER
    }
    
    public delegate ICacheProvider CacheProviderResolver(ENUM_CACHE_TYPE type);
    internal class CacheProviderInjector : IDependencyInjectorBase
    {
        /// <summary>
        /// SOP (State Oriented Programing Sample)
        /// </summary>
        private readonly Dictionary<ENUM_CACHE_TYPE, Func<IServiceProvider, ICacheProvider>> _cacheState =
            new Dictionary<ENUM_CACHE_TYPE, Func<IServiceProvider, ICacheProvider>>()
            {
                { ENUM_CACHE_TYPE.MEMORY, (s) => s.GetService<MemoryCacheProvider>() },
                { ENUM_CACHE_TYPE.REDIS, (s) => s.GetService<DistributeCacheProvider>() },
                { ENUM_CACHE_TYPE.LITEDB, (s) => s.GetService<LiteDbCacheProvider>() },
                { ENUM_CACHE_TYPE.FASTER, (s) => s.GetService<MSFasterCacheProvider>() },
            };
        
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            //AddMemoryCache 선언에 의해 MemoryCache 사용
            services.AddMemoryCache()
                //아래 선언에 의해 DistributeCache가 Redis로 지정됨.
                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["RedisConfiguration:Uri"];
                })
                .AddSingleton<MemoryCacheProvider>()
                .AddSingleton<DistributeCacheProvider>()
                .AddSingleton<LiteDbCacheProvider>()
                .AddSingleton<MSFasterCacheProvider>()
                .AddSingleton<CacheProviderResolver>(provider => key =>
                {
                    var func = _cacheState[key];
                    if (func.xIsEmpty()) throw new NotImplementedException($"key{key.ToString()} not implemented");
                    return func(provider);
                });
        }
    }

    public static class CacheProviderInjectorExtension
    {
        public static void AddCacheProviderInjector(this IServiceCollection services, IConfiguration configuration)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new CacheProviderInjector()
            }, services, configuration);
            diCore.Inject();
        }
    }
 
}