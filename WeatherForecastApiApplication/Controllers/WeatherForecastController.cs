using System.Collections.Generic;
using Application.Interfaces.WeahterForecast;
using Domain.Entities;
using Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<WeatherForecast> Get()
        {
            return _weatherForecastService.GetAllWeatherForecast();
        }
    }
}