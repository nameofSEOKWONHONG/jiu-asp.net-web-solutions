using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Entities.WeatherForecast;
using Domain.Response;
using WeatherForecastService.Services;

namespace WeatherForecastService.Features.Queries
{
    public record GetWeatherForecastDataQuery(string Summary) : IRequest<Result<TB_WEATHERFORECAST>>;

    public class GetWeatherForecastDataHandler : IRequestHandler<GetWeatherForecastDataQuery, Result<TB_WEATHERFORECAST>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public GetWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        
        public Task<Result<TB_WEATHERFORECAST>> Handle(GetWeatherForecastDataQuery request, CancellationToken cancellationToken)
        {
            var response = this._weatherForcastService.GetWeatherForecast(request.Summary);

            return Result<TB_WEATHERFORECAST>.SuccessAsync(response);
        }
    }
}