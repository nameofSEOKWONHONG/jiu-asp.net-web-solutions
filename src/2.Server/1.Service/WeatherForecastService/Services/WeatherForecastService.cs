using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Domain.Entities;
using Application.Infrastructure.Cache;
using Domain.Entities.WeatherForecast;
using Domain.Enums;
using eXtensionSharp;

namespace WeatherForecastService.Services
{
    /// <summary>
    /// Litedb example
    /// </summary>
    public class WeatherForcastService : IWeatherForcastService {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IEnumerable<TB_WEATHERFORECAST> _weatherForcasts;

        private readonly ILiteDatabase _database;
        private readonly ILiteCollection<TB_WEATHERFORECAST> _weatherForecastCollection;
        private readonly ICacheProvider _cacheProvider;
        public WeatherForcastService(CacheProviderResolver cacheProviderResolver)
        {
            _cacheProvider = cacheProviderResolver(ENUM_CACHE_TYPE.MEMORY);
            
            DateTime startDate = DateTime.Now;
            _weatherForcasts = Enumerable.Range(1, 5).Select(index => new TB_WEATHERFORECAST
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
            _weatherForecastCollection = _database.GetCollection<TB_WEATHERFORECAST>();
        }

        public IEnumerable<TB_WEATHERFORECAST> GetAllWeatherForecast()
        {
            var result = _cacheProvider.GetCache<IEnumerable<TB_WEATHERFORECAST>>();
            if (result.xIsEmpty())
            {
                result = _weatherForecastCollection.FindAll().ToList();
                if (result.xIsNotEmpty())
                {
                    _cacheProvider.SetCache<IEnumerable<TB_WEATHERFORECAST>>(result);
                }
            }

            return result;
        }

        public void CreateBaseData()
        {
            _weatherForecastCollection.DeleteAll();
            _weatherForecastCollection.InsertBulk(_weatherForcasts);
        }

        public TB_WEATHERFORECAST GetWeatherForecast(string summary)
        {
            var result = _cacheProvider.GetCache<TB_WEATHERFORECAST>();
            if (result == null)
            {
                result = _weatherForecastCollection.FindOne(m => m.Summary == summary);
                if (result != null)
                {
                    _cacheProvider.SetCache<TB_WEATHERFORECAST>(result);
                }
            }

            return result;
        }

        public void SaveWeatherForecast(TB_WEATHERFORECAST tbWeatherforecast)
        {
            _weatherForecastCollection.Insert(tbWeatherforecast);
        }
    }
}
