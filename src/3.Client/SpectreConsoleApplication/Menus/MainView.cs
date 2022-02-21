﻿using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.Member;
using SpectreConsoleApplication.Menus.WeatherForecast;

namespace SpectreConsoleApplication.Menus;

public class MainView
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;

    private readonly Dictionary<string, Func<IServiceProvider, IViewBase>> _menuViewStates = new() 
    {
        {"Counter", (s) => s.GetRequiredService<CounterView>()},
        {"WeatherForecast", (s) => s.GetRequiredService<WeatherForecastView>()},
        {"Member", (s) => s.GetRequiredService<MemberView>()}
    };
    
    public MainView(ILogger<MainView> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }
    
    public void Run()
    {
        CONTINUE:
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
                    "Counter", "WeatherForecast", "Member", "Exit"
                }));
        var menu = menus.Count == 1 ? menus.First() : null;
        if(menu.xIsEquals("Exit")) return;
        
        var menuViewState = _menuViewStates[menu];
        if (menuViewState.xIsEmpty()) AnsiConsole.WriteLine("select try again...");
        var menuView = menuViewState(_services);
        menuView.Show();
        goto CONTINUE;
    }
}




