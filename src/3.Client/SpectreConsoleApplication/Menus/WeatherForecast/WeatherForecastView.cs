using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastView : MenuViewBase
{
    private readonly WeatherForecastAction _action;
    
    public WeatherForecastView(ILogger<WeatherForecastView> logger,
        WeatherForecastAction action) : base(logger)
    {
        _action = action;
    }

    public override void Show()
    {
        CONTINUE:
        var items = _action.GetWeatherForecastList();
        var table = new Table();
        AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .Start(context =>
            {
                items.xForEach((item, i) =>
                {
                    var kv = item.xToDictionary();
                    if (i == 0) 
                    {
                        table.AddColumns(kv.Keys.ToArray());
                        context.Refresh();
                        Thread.Sleep(100);                        
                    }

                    table.AddRow(kv.Values.ToArray());
                    context.Refresh();
                });
            });

        var result = AnsiConsole.Ask<bool>("exit : ", true);
        if (!result) goto CONTINUE;
    }
}