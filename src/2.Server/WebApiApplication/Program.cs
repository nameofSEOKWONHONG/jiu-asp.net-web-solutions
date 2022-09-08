using System;
using System.Reflection;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace WebApiApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            
            try
            {
                TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel(options => options.ListenAnyIP(5001));
                    webBuilder.UseKestrel(opts =>
                    {
                        opts.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                        opts.Limits.MaxConcurrentConnections = 100;
                        opts.Limits.MaxConcurrentUpgradedConnections = 100;
                        opts.Limits.MaxRequestBodySize = 100_000_000;
                        opts.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configuration.AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        optional: true, reloadOnChange: true);

                    #region #region [custom json config]

                    configuration.AddJsonFile("./ConfigFiles/filtersettings.json", true, true);

                    #endregion
                })
                .UseSerilog();

        /// <summary>
        /// <see href="https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker"/>
        /// </summary>
        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
#if RELEASE
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
#endif
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        
        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }        
    }
}