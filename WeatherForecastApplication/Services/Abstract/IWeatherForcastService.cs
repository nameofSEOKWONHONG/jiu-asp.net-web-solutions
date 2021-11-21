﻿using System.Collections.Generic;
using Domain.Entities;

namespace WeatherForecastApplication.Services.Abstract
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> GetAll();
        void CreateBaseData();
        WeatherForecast GetWeatherForecast(string summary);
        void SaveWeatherForecast(WeatherForecast weatherForcast);
    }
}