﻿using System.Threading;
using System.Threading.Tasks;
using Application.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class RedisBackgroundService : BackgroundServiceBase
    {
        public RedisBackgroundService(ILogger<RedisBackgroundService> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory)
        {
            
        }

        protected override Task OnRunAsync(CancellationToken stopingToken)
        {
            throw new NotImplementedException();
        }
    }
}