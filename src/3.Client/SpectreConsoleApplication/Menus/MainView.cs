using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.WeatherForecast;

namespace SpectreConsoleApplication.Menus;

public class MainView
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;

    private readonly Dictionary<string, Func<IServiceProvider, IMenuViewBase>> _menuStates = new() 
    {
        {"Counter", (s) => s.GetRequiredService<CounterView>()},
        {"WeatherForecast", (s) => s.GetRequiredService<WeatherForecastView>()},
    };
    
    public MainView(ILogger<MainView> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }
    
    public void Run()
    {
        INIT:
        var menus = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Title("[green]select menu[/]?")
                // .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                // .InstructionsText("[grey](Press [blue][/] to toggle a menu, [green][/] to accept)[/]")
                // .AddChoiceGroup("Conuter", new[]
                // {
                //     "Blackcurrant", "Blueberry", "Cloudberry",
                //     "Elderberry", "Honeyberry", "Mulberry"
                // })
                .AddChoices(new[]
                {
                    "Counter", "WeatherForecast"
                }));
        var menu = menus.Count == 1 ? menus.First() : null;
        if (menu.xIsEmpty()) goto INIT;

        var impl = _menuStates[menu];
        if (impl.xIsEmpty()) AnsiConsole.WriteLine("select try again...");
        var menuImpl = impl(_services);
        menuImpl.Show();
        goto INIT;
    }
}




