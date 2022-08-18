using MailSendWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //TODO : 서비스를 등록해보자.
        services.AddHostedService<MailSendWorker>();
    })
    .Build();

await host.RunAsync();