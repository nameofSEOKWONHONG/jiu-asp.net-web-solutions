using ClientApplication.Services;
using Domain.Base;
using Domain.Entities.WeatherForecast;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public interface IWeatherForecastAction
{
    TB_WEATHERFORECAST SelectedItem { get; set; }
    IEnumerable<TB_WEATHERFORECAST> Items { get; }
    bool Save();
}

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IWeatherForecastAction))]
public sealed class WeatherForecastAction : ActionBase, IWeatherForecastAction
{
    private IEnumerable<TB_WEATHERFORECAST> _items;
    public IEnumerable<TB_WEATHERFORECAST> Items
    {
        get
        {
            _items.xIfEmpty(() =>
            {
                _items = _weatherForecastService.GetsForecastAsync().GetAwaiter().GetResult();
            });

            return _items;
        }
    }
    
    public TB_WEATHERFORECAST SelectedItem { get; set; }

    private readonly IWeatherForecastService _weatherForecastService;
    public WeatherForecastAction(ILogger<WeatherForecastAction> logger,
        IContextBase contextBase,
        IHttpClientFactory clientFactory,
        IWeatherForecastService service) : base(logger, contextBase, clientFactory)
    {
        _weatherForecastService = service;
    }

    public bool Save()
    {
        if (SelectedItem.xIsEmpty()) throw new Exception("selected item is null");
        //insert or update
        SelectedItem = null;
        return true;
    }

    public override void Dispose()
    {
        Console.WriteLine($"{nameof(WeatherForecastAction)} dispose");
    }
}