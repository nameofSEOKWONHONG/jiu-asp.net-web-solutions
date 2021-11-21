using System.Threading;
using System.Threading.Tasks;
using Application.Response;
using Domain.Entities;
using MediatR;
using WeatherForecastApplication.Services.Abstract;

namespace WeatherForecastApplication.Features.Command
{
    public class SaveWeatherForecastCommand : IRequest<Result<int>>
    {
        public WeatherForecast WeatherForecast { get; set; }
    }
    
    public class SaveWeatherForecastDataHandler : IRequestHandler<SaveWeatherForecastCommand, Result<int>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public SaveWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<Result<int>> Handle(SaveWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            _weatherForcastService.SaveWeatherForecast(request.WeatherForecast);
            return Result<int>.SuccessAsync(1);
        }
    }
}