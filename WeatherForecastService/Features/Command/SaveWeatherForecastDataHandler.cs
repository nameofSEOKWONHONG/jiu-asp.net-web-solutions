﻿using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.WeahterForecast;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace WeatherForecastService.Features.Command
{
    public record SaveWeatherForecastCommand(WeatherForecast weatherForecast) : IRequest<Result<int>>;
    
    public class SaveWeatherForecastDataHandler : IRequestHandler<SaveWeatherForecastCommand, Result<int>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public SaveWeatherForecastDataHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<Result<int>> Handle(SaveWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            _weatherForcastService.SaveWeatherForecast(request.weatherForecast);
            return Result<int>.SuccessAsync(1);
        }
    }
}