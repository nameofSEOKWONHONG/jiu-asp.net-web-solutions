using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Entities;
using Application.Response;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Features.WeatherForecasts.Queries.GetData
{
    public record GetWeatherForecastDataQuery(string Summary) : IRequest<Result<WeatherForecast>>;

    internal class GetWeatherForecastDataHandler : IRequestHandler<GetWeatherForecastDataQuery, Result<WeatherForecast>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public GetWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        
        public async Task<Result<WeatherForecast>> Handle(GetWeatherForecastDataQuery request, CancellationToken cancellationToken)
        {
            var response = this._weatherForcastService.GetWeatherForecast(request.Summary);

            return await Result<WeatherForecast>.SuccessAsync(response);
        }
    }
}