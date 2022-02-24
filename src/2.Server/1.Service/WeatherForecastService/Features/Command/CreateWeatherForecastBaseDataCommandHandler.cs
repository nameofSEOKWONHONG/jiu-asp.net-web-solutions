using System.Threading;
using System.Threading.Tasks;
using Domain.Response;
using eXtensionSharp;
using MediatR;
using WeatherForecastService.Services;

namespace WeatherForecastService.Features.Command
{
    public class CreateWeatherForecastBaseDataCommand : IRequest<ResultBase<int>>
    {
        
    }
    
    public class CreateWeatherForecastBaseDataCommandHandler : IRequestHandler<CreateWeatherForecastBaseDataCommand, ResultBase<int>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public CreateWeatherForecastBaseDataCommandHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<ResultBase<int>> Handle(CreateWeatherForecastBaseDataCommand request, CancellationToken cancellationToken)
        {
            var data = this._weatherForcastService.GetAllWeatherForecast();
            if (data.xIsNotEmpty()) return ResultBase<int>.FailAsync("data exists");
            this._weatherForcastService.CreateBaseData();
            return ResultBase<int>.SuccessAsync(5);
        }
    }
}