using System.Collections.Generic;
using Domain.Entities;
using Domain.Entities.WeatherForecast;

namespace WeatherForecastService.Services
{
    public interface IWeatherForcastService
    {
        IEnumerable<TB_WEATHERFORECAST> GetAllWeatherForecast();
        void CreateBaseData();
        TB_WEATHERFORECAST GetWeatherForecast(string summary);
        void SaveWeatherForecast(TB_WEATHERFORECAST weatherForcast);
    }
}