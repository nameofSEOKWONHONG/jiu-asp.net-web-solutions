using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherForecastApplication.Services.Abstract;

namespace WeatherForecastApiApplication.Controllers
{
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly IWeatherForcastService _weatherForecastService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IWeatherForcastService weatherForecastService) : base(logger)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return _weatherForecastService.GetAllWeatherForecast();
        }
    }
}