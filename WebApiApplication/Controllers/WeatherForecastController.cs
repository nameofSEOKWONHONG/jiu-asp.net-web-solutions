using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Infrastructure.Abstract;
using WeatherForecastApplication.Features.Command;
using WeatherForecastApplication.Features.Queries;

namespace WebApiApplication.Controllers {
    public class WeatherForecastController : ApiControllerBase<WeatherForecastController> {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await this._mediator.Send(new GetAllWeatherForecastDataQuery());
            return Ok(result);
        }
        
        [HttpGet("{summary}")]
        public async Task<IActionResult> Get(string summary)
        {
            var result = await this._mediator.Send(new GetWeatherForecastDataQuery(summary));
            return Ok(result);
        }

        [HttpPost("CreateBaseData")]
        public async Task<IActionResult> CreateBaseData()
        {
            var result = await this._mediator.Send(new CreateWeatherForecastBaseDataCommand());
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Save(WeatherForecast weatherForecast)
        {
            var result = await this._mediator.Send(new SaveWeatherForecastCommand(weatherForecast));
            return Ok(result);
        }
    }
}