using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using WebApiApplication.Services;
using WebApiApplication.SharedLibrary.Entities;

namespace WebApiApplication.Controllers {
    public class WeatherForecastController : ApiControllerBase{
        private readonly IWeatherForcastService _weatherForcastService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForcastService weatherForcastService) : base(logger) {
            this._weatherForcastService = weatherForcastService;
            base.logger.LogInformation($"{nameof(WeatherForecastController)} created time {DateTime.Now.ToLongDateString()}");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> GetAll()
        {
            return _weatherForcastService.GetAll();
        }
        
        [HttpGet("{summary}")]
        public WeatherForecast Get(string summary) {
            return this._weatherForcastService.GetWeatherForecast(summary);
        }

        [HttpPost("CreateBaseData")]
        public void CreateBaseData()
        {
            this._weatherForcastService.CreateBaseData();
        }
        [HttpPost]
        public void Save(WeatherForecast weatherForecast) {
            this._weatherForcastService.SaveWeatherForecast(weatherForecast);
        }
    }
}