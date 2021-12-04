using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundServices
{
    public class RedisBackgroundService : BackgroundService
    {
        public RedisBackgroundService()
        {
            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}