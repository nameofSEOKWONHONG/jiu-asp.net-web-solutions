using Application;
using eXtensionSharp;
using TodoService;
using WorkerApplication;

/*
 * <see href="https://docs.microsoft.com/ko-kr/dotnet/core/extensions/workers"/>
 * linux deploy : https://dev.to/uthmanrahimi/deploy-net-core-worker-service-on-linux-1mjc
 * windows deploy : https://docs.microsoft.com/ko-kr/dotnet/core/extensions/windows-service
 * Microsoft.Extensions.Hosting.WindowsServices는 사용하지 않는다. sc.exe를 이용하면 dll로도 운영 가능.
*/
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
        services.AddApplicationInjector(hostContext.Configuration);
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