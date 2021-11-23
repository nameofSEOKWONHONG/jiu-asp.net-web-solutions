using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Cache;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class CacheResetBackgroundService : BackgroundService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger _logger;
        private readonly int _interval = (1000 * 60) * 60;
        
        public CacheResetBackgroundService(ILogger<CacheResetBackgroundService> logger,
            CacheProviderResolver cacheProviderResolver)
        {
            _cacheProvider = cacheProviderResolver(ENUM_CACHE_TYPE.MEMORY);
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"reset start : {_cacheProvider.Count}");
                _cacheProvider.Reset();
                _logger.LogInformation($"reset end : {_cacheProvider.Count}");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}