using System.Collections.Generic;
using Domain.Entities;
using Domain.Entities.WeatherForecast;
using Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastService.Services;

namespace WeatherForecastApiApplication.Controllers
{
    public class WeatherForecastController : ApiControllerBase<WeatherForecastController>
    {
        private readonly IWeatherForcastService _weatherForecastService;
        public WeatherForecastController(IWeatherForcastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<TB_WEATHERFORECAST> Get()
        {
            return _weatherForecastService.GetAllWeatherForecast();
        }
    }
}