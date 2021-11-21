using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Entities;
using Application.Response;
using WeatherForecastApplication.Services.Abstract;

namespace WeatherForecastApplication.Features.Queries
{
    public record GetWeatherForecastDataQuery(string Summary) : IRequest<Result<WeatherForecast>>;

    public class GetWeatherForecastDataHandler : IRequestHandler<GetWeatherForecastDataQuery, Result<WeatherForecast>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public GetWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        
        public Task<Result<WeatherForecast>> Handle(GetWeatherForecastDataQuery request, CancellationToken cancellationToken)
        {
            var response = this._weatherForcastService.GetWeatherForecast(request.Summary);

            return Result<WeatherForecast>.SuccessAsync(response);
        }
    }
}