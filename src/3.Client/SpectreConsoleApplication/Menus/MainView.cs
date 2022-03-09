using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SpectreConsoleApplication.Menus;

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton)]
public class MainView
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;
    private readonly IMainAction _mainAction;

    public MainView(ILogger<MainView> logger, IServiceProvider services, IMainAction action)
    {
        _logger = logger;
        _services = services;
        _mainAction = action;
    }
    
    public bool Show()
    {
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
                .AddChoices(_mainAction.GetMenuNames()));
        var selectedMenu = menus.Count == 1 ? menus.First() : null;
        try
        {
            var selectedView = _mainAction.GetView(selectedMenu, _services);
            if (selectedView.xIsNotEmpty())
            {
                selectedView.RunLifeTime();
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e, ExceptionFormats.Default);
        }

        return true;
    }
}




