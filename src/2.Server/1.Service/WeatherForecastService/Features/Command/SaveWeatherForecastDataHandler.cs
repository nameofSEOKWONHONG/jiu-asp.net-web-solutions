using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.WeatherForecast;
using Domain.Response;
using MediatR;
using WeatherForecastService.Services;

namespace WeatherForecastService.Features.Command
{
    public record SaveWeatherForecastCommand(TB_WEATHERFORECAST TbWeatherforecast) : IRequest<ResultBase<int>>;
    
    public class SaveWeatherForecastDataHandler : IRequestHandler<SaveWeatherForecastCommand, ResultBase<int>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public SaveWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<ResultBase<int>> Handle(SaveWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            _weatherForcastService.SaveWeatherForecast(request.TbWeatherforecast);
            return ResultBase<int>.SuccessAsync(1);
        }
    }
}