using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.WeatherForecast;

namespace SpectreConsoleApplication.Menus;

public class MenuView
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;

    private readonly Dictionary<string, Func<IServiceProvider, IMenuViewBase>> _menuStates = new()
    {
        {"Counter", (s) => s.GetRequiredService<CounterView>()},
        {"WeatherForecast", (s) => s.GetRequiredService<WeatherForecastView>()},
    };
    
    public MenuView(ILogger<MenuView> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }
    
    public void Run()
    {
        START:
        var menus = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Title("What are your [green]select menu[/]?")
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
        if (menu.xIsEmpty()) goto START;

        var impl = _menuStates[menu];
        if (impl.xIsEmpty()) AnsiConsole.WriteLine("select try again...");
        var menuImpl = impl(_services);
        menuImpl.Show();
        goto START; 
    }
}




