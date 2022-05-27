// See https://aka.ms/new-console-template for more information

using CustomOptionSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<MyConfigService>();
        services.AddSingleton<CustomSettingService>();
        
        var appsetting = new ConfigurationBuilder()
            .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true)
            .Build();
        services.Configure<MyConfig>(appsetting.GetSection(nameof(MyConfig)));
        
        var customsetting = new ConfigurationBuilder()
            .AddJsonFile("customsetting.json", optional: false, reloadOnChange: true)
            .Build();
        services.Configure<CustomSetting>(customsetting.GetSection(nameof(CustomSetting)));
        
    }).UseConsoleLifetime();
 
var host = builder.Build();
await host.StartAsync();

using (var serviceScope = host.Services.CreateScope())
{
    var svc = serviceScope.ServiceProvider.GetService<MyConfigService>();
    CONTINUE:
    var config = svc.Get();
    var v = JsonConvert.SerializeObject(config);
    Console.WriteLine(v);
    Console.WriteLine("enter yn");
    var yn = Console.ReadLine();
    if (yn.ToUpper() == "Y")
    {
        goto CONTINUE;
    }
}

await host.StopAsync();


