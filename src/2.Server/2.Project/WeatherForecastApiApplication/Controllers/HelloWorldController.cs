using HelloWorldService.Features.Queries;
using Infrastructure.Abstract.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace WeatherForecastApiApplication.Controllers
{
    public class HelloWorldController : ApiControllerBase<HelloWorldController>
    {
        [HttpGet("GetHelloWorld")]
        public IActionResult GetHelloWorld()
        {
            var result = _mediator.Send(new GetHelloWorldDataQuery()).GetAwaiter().GetResult();
            return Ok(result);
        }
    }
}