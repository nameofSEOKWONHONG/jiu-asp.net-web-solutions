using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Entities;
using WebApiApplication.Services;

namespace WebApiApplication.Controllers {
    public class WeatherForcastController : ApiControllerBase{
        private readonly IWeatherForcastService _weatherForcastService;
        public WeatherForcastController(ILogger<WeatherForcastController> logger, IWeatherForcastService weatherForcastService) : base(logger) {
            this._weatherForcastService = weatherForcastService;
        }

        [HttpGet("{name}")]
        public WeatherForcast Get(string name) {
            return this._weatherForcastService.GetWeatherForcast(name);
        }

        [HttpPost]
        public void Save(WeatherForcast weatherForcast) {
            this._weatherForcastService.SaveWeatherForcast(weatherForcast);
        }
    }
}