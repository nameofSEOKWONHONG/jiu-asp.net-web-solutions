using Application.Script.SharpScript;
using JWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<SharpScriptLoader>();
        services.AddHostedService<MainWorker>();
    })
    .Build();

await host.RunAsync();