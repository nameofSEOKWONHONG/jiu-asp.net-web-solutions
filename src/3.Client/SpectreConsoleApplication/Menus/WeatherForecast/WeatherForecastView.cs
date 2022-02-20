using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastView : MenuViewBaseBase
{
    private readonly WeatherForecastAction _action;
    
    public WeatherForecastView(ILogger<WeatherForecastView> logger,
        WeatherForecastAction action) : base(logger)
    {
        _action = action;
    }

    public override void Show()
    {
        //TODO : try implement self...
    }
}