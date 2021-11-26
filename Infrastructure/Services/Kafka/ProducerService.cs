using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Kafka
{
    public class ProducerService
    {
        private readonly IConfiguration _configuration;

        public ProducerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task CreateProducer()
        {
            var config = new ProducerConfig { BootstrapServers = _configuration["KafkaConfiguration:Uri"] };
            
            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("test", new Message<Null, string> { Value="test" });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        } 
    }
}