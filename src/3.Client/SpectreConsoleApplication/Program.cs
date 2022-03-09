// See https://aka.ms/new-console-template for more information

using ClientApplication.Services;
using ClientApplication.ViewModel;
using Domain.Base;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using SpectreConsoleApplication;
using SpectreConsoleApplication.Menus;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {   
        services.AddHttpClient(ClientConst.CLIENT_NAME, config =>
        {
            config.BaseAddress = new Uri(ClientConst.BASE_URL);
        });
        
        services.AddLifeTime();

        #region [manual injection - not use]

        services.AddScoped<ILoginService, LoginService>();

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

AnsiConsole.Write(
    new Panel("2022-02-25 14:16:00 [blue]PRAY FOR[/] [yellow]UKRAINE[/]")
        .RoundedBorder());

var encText = "nQW+DbVAi08uBUNWmCIam8XOe+8lkXNyv/jq40Ft5AgDTeFe4S/o7vSN8EetcE+DVdWBflwugsJs1y2t8VXnmrU8rwZWrjw1oZqN1NtKVLr2H9hQM8evHlgkKczx6Sgnswi0I13mNdZzmG8cy1EDnY3ltLO7KlCl79hzB5EFcAAhVTDVy2N6eGi26BT6OXmoE8WJDZd2TYqL31tyPZbitjy6h3drlG36B3mAbqWfY0viJqdHbgS1P8i649V6zeJN/zxaxBRC8XcOrgTamz7gKAl9p71QoNk+Gqw3aecKCPWRQI8bshR4iNe8Wa56fLhZoDvanpozZ1L0UphwLEfkHUHflaQW208h9ZtCb7LdK8e7ccQhlqXMTv/ClXNnGfpFy0GJQOW7R6z7NatiqaC45SzEf034xGxWYyqt+moxfL0wL9Uc5O2Z0s/nB7LsxE6sjFUQ5mHBIjFsytw5XpFes17a5etPFaNi+5vOfg5ogo7K5bHcSQNOTNQM631LFROsrGLC9DP2Zi+EY8fxHlrBwvXeh7tMR35Z00YgrEWT0nOKA/dMpS3Yt0Oq2Vzaiqn+RNVJcG7jT0rV2QkKAfs9OQXaOA2Kjc0oVNh4TlA71/zTh0tNvW9F3YHdfEcB8DjmkYUwpjlMZQCI2tX9WYQcTkWx9XCNDQtSNFd1kuIHplfS1YPe4JXdhNdMfNyWvSE1warUnbGiIG42/+UIlYEW8ByvErm4lyJng5KhKk4pfVI=";
AnsiConsole.Write(
    new Panel(encText.xToDecryptString("b14ca5898a4e4142aace2ea2143a2410"))
        .RoundedBorder());
try
{
    using (var serviceScope = host.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var session = services.GetRequiredService<IClientSession>();
        using (var loginView = services.GetRequiredService<LoginView>())
        {
            loginView.Show();
            if (loginView.ViewResult.xIsTrue())
            {
                session.AccessToken = loginView.AccessToken;
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
    else return; //exit;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}
