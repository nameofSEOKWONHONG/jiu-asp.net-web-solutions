using Domain.Entities.WeatherForecast;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Utils;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastView : ViewBase
{
    private readonly WeatherForecastAction _action;
    
    public WeatherForecastView(ILogger<WeatherForecastView> logger,
        ISession session,
        WeatherForecastAction action) : base(logger, session)
    {
        _action = action;
    }

    public override void Show()
    {
        CONTINUE:
        var items = _action.Items;
        items = _action.Items;
        if (items.xIsEmpty())
        {
            AnsiConsole.Ask<string>("data is empty, exit", "Y");
            return;
        }
        TableUtil.TableDraw<TB_WEATHERFORECAST>(items);
        var result = AnsiConsole.Ask<bool>("exit : ", true);
        if (!result) goto CONTINUE;
    }

    public override void Dispose()
    {
        Console.WriteLine($"{nameof(WeatherForecastView)} dispose");
    }
}