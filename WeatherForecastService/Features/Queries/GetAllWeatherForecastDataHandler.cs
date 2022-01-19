using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.WeahterForecast;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace WeatherForecastApplication.Features.Queries
{
    public record GetAllWeatherForecastDataQuery() : IRequest<Result<IEnumerable<WeatherForecast>>>;
    public class GetAllWeatherForecastDataHandler : IRequestHandler<GetAllWeatherForecastDataQuery, Result<IEnumerable<WeatherForecast>>>
    {
        private readonly IWeatherForcastService _weatherForcastService;

        public GetAllWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<Result<IEnumerable<WeatherForecast>>> Handle(GetAllWeatherForecastDataQuery request, CancellationToken cancellationToken)
        {
            var response = this._weatherForcastService.GetAllWeatherForecast();

            return Result<IEnumerable<WeatherForecast>>.SuccessAsync(response);
        }
    }
}