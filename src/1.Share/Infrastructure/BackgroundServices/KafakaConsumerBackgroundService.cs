using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Base;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class KafakaConsumerBackgroundService : BackgroundServiceBase
    {
        private readonly ConsumerConfig _consumerConfig;
        public KafakaConsumerBackgroundService(ILogger<KafakaConsumerBackgroundService> logger,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, serviceScopeFactory)
        {
            _consumerConfig = new ConsumerConfig
            { 
                GroupId = "test-consumer-group",
                BootstrapServers = _configuration["KafkaConfiguration:Uri"],
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };            
        }
        
        protected override async Task OnRunAsync(CancellationToken stoppingToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                c.Subscribe("test");
                
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(stoppingToken);
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}