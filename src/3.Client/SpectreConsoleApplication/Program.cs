using ClientApplication.ViewModel;
using Domain.Base;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Spectre.Console;
using SpectreConsoleApplication.Menus;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5));
        var longTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(10));

        //ref : https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
        services.AddHttpClient(ClientConst.CLIENT_NAME, config =>
            {
                config.BaseAddress = new Uri(ClientConst.BASE_URL);
            }).AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromSeconds(5)))
            .AddTransientHttpErrorPolicy(
                policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
            .AddPolicyHandler(message => message.Method == HttpMethod.Get ? timeoutPolicy : longTimeoutPolicy)
            //httpclientfactory는 호출마다 새로운 instance반환.
            //아래 설정으로 수명이 만료되지 않은 호출의 경우 instance 재사용.
            //기본 수명 2분
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            // .ConfigurePrimaryHttpMessageHandler(() =>
            //     new HttpClientHandler()
            //     {
            //         AllowAutoRedirect = true,
            //         UseDefaultCredentials = true
            //     })
            ;
            
        
        services.AddLifeTimeBuild();

        #region [manual injection - not use]
        
        // #region [add action]
        //
        // services.AddScoped<LoginAction>();
        // services.AddScoped<WeatherForecastAction>();
        // services.AddScoped<MemberAction>();
        //
        // #endregion
        //
        // #region [add view]
        //
        // services.AddTransient<LoginView>();
        // services.AddTransient<MainView>();
        // services.AddSingleton<CounterView>();
        // services.AddTransient<WeatherForecastView>();
        // services.AddTransient<MemberView>();
        //
        // #endregion

        #endregion

    }).UseConsoleLifetime();
 
var host = builder.Build();

try
{
    using (var serviceScope = host.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var session = services.GetRequiredService<IContextBase>();
        using (var loginView = services.GetRequiredService<LoginView>())
        {
            loginView.Show();
            if (loginView.ViewResult.xIsTrue())
            {
                session.AuthorizeInfo.AccessToken = loginView.AccessToken;
            }
        }
    }


    CONTINUE:
    var isContinue = false;
    using (var serviceScope = host.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var menuView = services.GetRequiredService<MainView>();
        isContinue = menuView.Show();
    }
    if (isContinue) goto CONTINUE;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}
