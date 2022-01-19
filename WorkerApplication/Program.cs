using Application;
using eXtensionSharp;
using TodoService;
using WorkerApplication;

//<see href="https://docs.microsoft.com/ko-kr/dotnet/core/extensions/workers"/>
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDatabaseInjector(hostContext.Configuration);
        services.AddTodoInjector();
        services.Configure<HostOptions>(option =>
        {
            option.ShutdownTimeout = TimeSpan.Parse(hostContext.Configuration["Options:ShutdownTimeout"])
                .xValue<TimeSpan>(TimeSpan.FromSeconds(5));
        }).AddHostedService<SampleWorker1>()
            .AddHostedService<SampleWorker2>();
    })
    .Build();

await host.RunAsync();