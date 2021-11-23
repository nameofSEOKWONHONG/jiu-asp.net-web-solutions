using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Domain.Entities;
using Application.Infrastructure.Cache;
using Application.Interfaces.WeahterForecast;
using eXtensionSharp;

namespace WeatherForecastApplication.Services
{
    public class WeatherForcastService : IWeatherForcastService {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IEnumerable<WeatherForecast> _weatherForcasts;

        private readonly ILiteDatabase _database;
        private readonly ILiteCollection<WeatherForecast> _weatherForecastCollection;
        private readonly ICacheProvider _cacheProvider;
        public WeatherForcastService(CacheProviderResolver cacheProviderResolver)
        {
            _cacheProvider = cacheProviderResolver(ENUM_CACHE_TYPE.MEMORY);
            
            DateTime startDate = DateTime.Now;
            _weatherForcasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

            _database = new LiteDatabase(new ConnectionString()
            {
                Filename = "weather_forecast.db",
                Connection = ConnectionType.Shared
                //Connection = ConnectionType.Direct
            });
            _weatherForecastCollection = _database.GetCollection<WeatherForecast>();
        }

        public IEnumerable<WeatherForecast> GetAllWeatherForecast()
        {
            var result = _cacheProvider.GetCache<IEnumerable<WeatherForecast>>();
            if (result.xIsEmpty())
            {
                result =_weatherForecastCollection.FindAll();
                if (result.xIsNotEmpty())
                {
                    _cacheProvider.SetCache<IEnumerable<WeatherForecast>>(result);
                }
            }

            return result;
        }

        public void CreateBaseData()
        {
            _weatherForecastCollection.DeleteAll();
            _weatherForecastCollection.InsertBulk(_weatherForcasts);
        }

        public WeatherForecast GetWeatherForecast(string summary)
        {
            var result = _cacheProvider.GetCache<WeatherForecast>();
            if (result == null)
            {
                result = _weatherForecastCollection.FindOne(m => m.Summary == summary);
                if (result != null)
                {
                    _cacheProvider.SetCache<WeatherForecast>(result);
                }
            }

            return result;
        }

        public void SaveWeatherForecast(WeatherForecast weatherForecast)
        {
            _weatherForecastCollection.Insert(weatherForecast);
        }
    }
}
