using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.WeatherForecast;
using Domain.Response;
using MediatR;
using WeatherForecastService.Services;

namespace WeatherForecastService.Features.Queries
{
    public record GetAllWeatherForecastDataQuery() : IRequest<Result<IEnumerable<TB_WEATHERFORECAST>>>;
    public class GetAllWeatherForecastDataHandler : IRequestHandler<GetAllWeatherForecastDataQuery, Result<IEnumerable<TB_WEATHERFORECAST>>>
    {
        private readonly IWeatherForcastService _weatherForcastService;

        public GetAllWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<Result<IEnumerable<TB_WEATHERFORECAST>>> Handle(GetAllWeatherForecastDataQuery request, CancellationToken cancellationToken)
        {
            var response = this._weatherForcastService.GetAllWeatherForecast();

            return Result<IEnumerable<TB_WEATHERFORECAST>>.SuccessAsync(response);
        }
    }
}