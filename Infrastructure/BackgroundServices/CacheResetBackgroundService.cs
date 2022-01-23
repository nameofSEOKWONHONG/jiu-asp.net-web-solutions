﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Cache;
using Domain.Configuration;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices
{
    public class CacheResetBackgroundService : BackgroundServiceBase
    {
        private readonly CacheProviderResolver _cacheProviderResolver;
        private readonly int _interval = (1000 * 60) * 60;
        private readonly CacheConfig _cacheOption;

        private readonly Dictionary<ENUM_CACHE_TYPE, Func<CacheProviderResolver, ICacheProvider>> _cacheStates =
            new Dictionary<ENUM_CACHE_TYPE, Func<CacheProviderResolver, ICacheProvider>>()
            {
                {ENUM_CACHE_TYPE.MEMORY, resolver => resolver(ENUM_CACHE_TYPE.MEMORY)},
                {ENUM_CACHE_TYPE.REDIS, resolver => resolver(ENUM_CACHE_TYPE.REDIS)},
                {ENUM_CACHE_TYPE.LITEDB, resolver => resolver(ENUM_CACHE_TYPE.LITEDB)},
                {ENUM_CACHE_TYPE.FASTER, resolver => resolver(ENUM_CACHE_TYPE.FASTER)},
                {ENUM_CACHE_TYPE.ROCKS, resolver => resolver(ENUM_CACHE_TYPE.ROCKS)}
            };

        public CacheResetBackgroundService(ILogger<CacheResetBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            CacheProviderResolver cacheProviderResolver) : base(logger, serviceScopeFactory)
        {
            _cacheProviderResolver = cacheProviderResolver;
        }

        protected override async Task Execute(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(CacheResetBackgroundService)} start");
                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var config = scope.ServiceProvider.GetService<IOptionsMonitor<CacheConfig>>()?.CurrentValue;
                    if (config.IsCacheReset)
                    {
                        _logger.LogInformation($"{nameof(CacheResetBackgroundService)} running");
                        config.ResetCacheTypes.xForEach(type =>
                        {
                            _cacheStates[type](_cacheProviderResolver).Reset();
                        });
                    }
                }
                _logger.LogInformation($"{nameof(CacheResetBackgroundService)} end");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}