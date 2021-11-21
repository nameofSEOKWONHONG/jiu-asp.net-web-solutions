﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Response;
using eXtensionSharp;
using MediatR;
using WeatherForecastApplication.Services.Abstract;

namespace WeatherForecastApplication.Features.Command
{
    public class CreateWeatherForecastBaseDataCommand : IRequest<Result<int>>
    {
        
    }
    
    public class CreateWeatherForecastBaseDataCommandHandler : IRequestHandler<CreateWeatherForecastBaseDataCommand, Result<int>>
    {
        private readonly IWeatherForcastService _weatherForcastService;
        public CreateWeatherForecastBaseDataCommandHandler(IWeatherForcastService weatherForcastService)
        {
            this._weatherForcastService = weatherForcastService;
        }
        public Task<Result<int>> Handle(CreateWeatherForecastBaseDataCommand request, CancellationToken cancellationToken)
        {
            var data = this._weatherForcastService.GetAllWeatherForecast();
            if (data.xIsNotEmpty()) return Result<int>.FailAsync("data exists");
            this._weatherForcastService.CreateBaseData();
            return Result<int>.SuccessAsync(5);
        }
    }
}