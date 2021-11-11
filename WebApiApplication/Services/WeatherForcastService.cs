using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using MongoDB.Bson;
using Realms;
using WebApiApplication.Infrastructure;
using WebApiApplication.SharedLibrary.Entities;

namespace WebApiApplication.Services {
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> GetAll();
        void CreateBaseData();
        WeatherForecast GetWeatherForecast(string summary);
        void SaveWeatherForecast(WeatherForecast weatherForcast);
    }

    public class WeatherForcastService : IWeatherForcastService {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IEnumerable<WeatherForecast> _weatherForcasts;

        private readonly ILiteDatabase _database;
        private readonly ILiteCollection<WeatherForecast> _weatherForecastCollection;
        private readonly CacheProvider _provider;
        public WeatherForcastService(CacheProvider provider)
        {
            _provider = provider;
            
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

        public IEnumerable<WeatherForecast> GetAll()
        {
            var result = _provider.GetCache<IEnumerable<WeatherForecast>>("weatherforecast_all");
            if (result == null)
            {
                result =_weatherForecastCollection.FindAll();
                if (result != null)
                {
                    _provider.SetCache<IEnumerable<WeatherForecast>>("weatherforecast_all", result);
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
            return _weatherForecastCollection.FindOne(m => m.Summary == summary);
        }

        public void SaveWeatherForecast(WeatherForecast weatherForecast)
        {
            _weatherForecastCollection.Insert(weatherForecast);
        }
    }
}
