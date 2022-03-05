using Application.Infrastructure.Injection;
using Domain.Entities.WeatherForecast;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Utils;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton)]
public sealed class WeatherForecastView : ViewBase
{
    private readonly IWeatherForecastAction _action;
    
    public WeatherForecastView(ILogger<WeatherForecastView> logger,
        IWeatherForecastAction action) : base(logger)
    {
        _action = action;
    }

    public override void Show()
    {
        var items = _action.Items;
        items = _action.Items;
        if (items.xIsEmpty())
        {
            AnsiConsole.Ask<string>("data is empty, exit", "Y");
            return;
        }
        TableUtil.TableDraw<TB_WEATHERFORECAST>(items);
    }

    public override void Dispose()
    {
        Console.WriteLine($"{nameof(WeatherForecastView)} dispose");
    }
}