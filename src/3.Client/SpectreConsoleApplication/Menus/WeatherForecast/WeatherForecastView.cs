using Domain.Entities.WeatherForecast;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastView : ViewBase
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
            .Start(ctx =>
            {
                var columns = new List<string>();
                typeof(TB_WEATHERFORECAST).GetProperties().xForEach(prop =>
                {
                    columns.Add(prop.Name);
                });
                table.AddColumns(columns.ToArray());
                ctx.Refresh();
                
                items.xForEach((item, i) =>
                {
                    var map = item.xToDictionary();
                    var row = new List<string>();
                    columns.xForEach(column =>
                    {
                        row.Add(map[column].xValue<string>());
                    });
                    table.AddRow(row.ToArray());
                    ctx.Refresh();
                });
            });

        var result = AnsiConsole.Ask<bool>("exit : ", true);
        if (!result) goto CONTINUE;
    }
}