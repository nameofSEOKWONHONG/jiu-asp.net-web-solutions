// See https://aka.ms/new-console-template for more information

using System.Diagnostics.Metrics;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using SpectreConsoleApplication;
using SpectreConsoleApplication.Menus;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.Member;
using SpectreConsoleApplication.Menus.WeatherForecast;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient(AppConst.HTTP_NAME, config =>
        {
            config.BaseAddress = new Uri(AppConst.HTTP_URL);
        });

        #region [add action]

        services.AddScoped<LoginAction>();
        services.AddScoped<WeatherForecastAction>();
        services.AddScoped<MemberAction>();

        #endregion

        #region [add view]

        services.AddTransient<LoginView>();
        services.AddTransient<MainView>();
        services.AddTransient<CounterView>();
        services.AddTransient<WeatherForecastView>();
        services.AddTransient<MemberView>();

        #endregion
    });
 
var host = builder.Build();

AnsiConsole.Write(
    new Panel("2022-02-25 14:16:00 [blue]PRAY FOR[/] [yellow]UKRAINE[/]")
        .RoundedBorder());

try
{
    using (var serviceScope = host.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        if (AppConst.ACESS_TOKEN.xIsEmpty())
        {
            var loginView = services.GetRequiredService<LoginView>();
            loginView.Show();

            if (loginView.AccessToken.xIsEmpty())
                throw new UnauthorizedAccessException("login failed.");
            
            AppConst.ACESS_TOKEN = loginView.AccessToken;
        }
    }

    CONTINUE:
    using (var serviceScope = host.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var menuView = services.GetRequiredService<MainView>();
        var isContinue = menuView.Show();
        if (isContinue.xIsFalse()) goto CONTINUE;
        else return; //exit;
    }
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

await host.RunAsync();
