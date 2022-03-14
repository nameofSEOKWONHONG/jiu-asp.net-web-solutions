using Domain.Base;
using Domain.Entities.WeatherForecast;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Utils;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

[AddService(ENUM_LIFE_TIME_TYPE.Singleton)]
public sealed class WeatherForecastView : ViewBase
{
    private readonly IWeatherForecastAction _action;
    
    public WeatherForecastView(ILogger<WeatherForecastView> logger,
        IClientSession clientSession,
        IWeatherForecastAction action) : base(logger, clientSession)
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