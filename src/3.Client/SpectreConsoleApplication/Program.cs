// See https://aka.ms/new-console-template for more information

using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using SpectreConsoleApplication;
using SpectreConsoleApplication.Menus;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.WeatherForecast;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();

        #region [add action]

        services.AddScoped<LoginAction>();
        services.AddScoped<WeatherForecastAction>();

        #endregion

        #region [add view]

        services.AddSingleton<LoginView>();
        services.AddSingleton<MainView>();
        services.AddSingleton<CounterView>();
        services.AddSingleton<WeatherForecastView>();

        #endregion
    }).UseConsoleLifetime();
 
var host = builder.Build();
using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
 
try
{
    var loginView = services.GetRequiredService<LoginView>();
    loginView.Show();
    if(loginView.AccessToken.xIsNotEmpty())
    {
        AppConst.AccessToken = loginView.AccessToken;
        var menuView = services.GetRequiredService<MainView>();
        menuView.Run();
    }
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}
