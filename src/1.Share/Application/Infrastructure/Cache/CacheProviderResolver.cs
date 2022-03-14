using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Application.Abstract;
using Application.Infrastructure.Cache.MSFaster;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure.Cache
{   
    public delegate ICacheProvider CacheProviderResolver(ENUM_CACHE_TYPE type);
    internal class CacheProviderInjector : IServiceInjectionBase
    {
        /// <summary>
        /// SOP (State Oriented Programing Sample)
        /// </summary>
        private readonly Dictionary<ENUM_CACHE_TYPE, Func<IServiceProvider, ICacheProvider>> _cacheStates =
            new()
            {
                { ENUM_CACHE_TYPE.MEMORY, (s) => s.GetService<MemoryCacheProvider>() },
                { ENUM_CACHE_TYPE.REDIS, (s) => s.GetService<DistributeCacheProvider>() },
                { ENUM_CACHE_TYPE.LITEDB, (s) => s.GetService<LiteDbCacheProvider>() },
                { ENUM_CACHE_TYPE.FASTER, (s) => s.GetService<MSFasterCacheProvider>() },
                { ENUM_CACHE_TYPE.ROCKS, (s) => s.GetService<RocksDbCacheProvider>()}
            };
        
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services
                //AddMemoryCache 선언에 의해 MemoryCache 사용
                .AddMemoryCache()
                //아래 선언에 의해 DistributeCache가 Redis로 지정됨.
                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["RedisConfiguration:Uri"];
                })
                .AddSingleton<MemoryCacheProvider>()
                .AddSingleton<DistributeCacheProvider>()
                .AddSingleton<LiteDbCacheProvider>()
                .AddSingleton<MSFasterCacheProvider>()
                .AddSingleton<RocksDbCacheProvider>()
                .AddSingleton<CacheProviderResolver>(provider => key =>
                {
                    var func = _cacheStates[key];
                    if (func.xIsEmpty()) throw new NotImplementedException($"key{key.ToString()} not implemented");
                    return func(provider);
                });
        }
    }
}