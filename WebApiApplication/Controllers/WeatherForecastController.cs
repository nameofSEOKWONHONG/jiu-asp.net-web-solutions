using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using WebApiApplication.Services;
using SharedLibrary.Entities;
using WebApiApplication.Features.WeatherForecasts.Queries.GetData;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers {
    public class WeatherForecastController : ApiControllerBase<WeatherForecastController> {
        private readonly IWeatherForcastService _weatherForcastService;
        public WeatherForecastController(IWeatherForcastService weatherForcastService) {
            this._weatherForcastService = weatherForcastService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> GetAll()
        {
            var user = this.SessionContext.User;
            return _weatherForcastService.GetAll();
        }
        
        [HttpGet("{summary}")]
        public async Task<IActionResult> Get(string summary)
        {
            var result = await this._mediator.Send(new GetWeatherForecastDataQuery(summary));
            return Ok(result);
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