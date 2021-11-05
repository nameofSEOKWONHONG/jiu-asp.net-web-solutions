using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using MongoDB.Bson;
using Realms;
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
        
        public WeatherForcastService()
        {
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
            return _weatherForecastCollection.FindAll();
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

        public void SaveWeatherForecast(WeatherForecast weatherForcast)
        {
            _weatherForecastCollection.Insert(weatherForcast);
        }
    }
}
