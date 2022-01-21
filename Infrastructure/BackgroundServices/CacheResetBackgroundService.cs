using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Cache;
using Infrastructure.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class CacheResetBackgroundService : BackgroundServiceBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly int _interval = (1000 * 60) * 60;
        
        public CacheResetBackgroundService(ILogger<CacheResetBackgroundService> logger,
            CacheProviderResolver cacheProviderResolver) : base(logger)
        {
            _cacheProvider = cacheProviderResolver(ENUM_CACHE_TYPE.MEMORY);
        }

        protected override async Task Execute(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"reset start : {_cacheProvider.Count}");
                // if (bool.TryParse(_configuration["MemoryCacheReset"], out bool isReset))
                // {
                //     _cacheProvider.Reset();
                // }
                _logger.LogInformation($"reset end : {_cacheProvider.Count}");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}