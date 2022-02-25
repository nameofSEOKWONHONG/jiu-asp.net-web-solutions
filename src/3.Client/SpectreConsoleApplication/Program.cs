// See https://aka.ms/new-console-template for more information

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
        services.AddHttpClient(AppConst.HTTP_NAME,config =>
        {
            config.BaseAddress = new Uri(AppConst.HTTP_URL);
        });

        #region [add action]

        services.AddScoped<LoginAction>();
        services.AddScoped<WeatherForecastAction>();
        services.AddScoped<MemberAction>();

        #endregion

        #region [add view]

        services.AddSingleton<LoginView>();
        services.AddSingleton<MainView>();
        services.AddSingleton<CounterView>();
        services.AddSingleton<WeatherForecastView>();
        services.AddSingleton<MemberView>();

        #endregion
    }).UseConsoleLifetime();
 
var host = builder.Build();
using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;

AnsiConsole.Write(
    new Panel("2022-02-25 14:16:00 [yellow]PRAY FOR[/] [blue]UKRAINE[/]")
        .RoundedBorder());
 
try
{
    var loginView = services.GetRequiredService<LoginView>();
    loginView.Show();
    
    if (loginView.AccessToken.xIsEmpty())
        throw new UnauthorizedAccessException("login failed.");
    
    AppConst.ACESS_TOKEN = loginView.AccessToken;
    var menuView = services.GetRequiredService<MainView>();
    menuView.Run();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}
