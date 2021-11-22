using System.Collections.Generic;
using Domain.Entities;

namespace Application.Interfaces.WeahterForecast
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> GetAllWeatherForecast();
        void CreateBaseData();
        WeatherForecast GetWeatherForecast(string summary);
        void SaveWeatherForecast(WeatherForecast weatherForcast);
    }
}