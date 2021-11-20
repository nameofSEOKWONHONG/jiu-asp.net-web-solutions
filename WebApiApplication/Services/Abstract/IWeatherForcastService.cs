﻿using System.Collections.Generic;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> GetAll();
        void CreateBaseData();
        WeatherForecast GetWeatherForecast(string summary);
        void SaveWeatherForecast(WeatherForecast weatherForcast);
    }
}