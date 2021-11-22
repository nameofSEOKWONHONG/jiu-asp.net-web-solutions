using System.Collections.Generic;
using System.Reflection;
using Application.Abstract;
using eXtensionSharp;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecastApplication;

namespace WeatherForecastApiApplication
{
    public static class ApplicationInjector
    {
        public static void AddInject(this IServiceCollection services)
        {
            services.AddWeatherForecastApplicationInjector();
            services.AddWeatherForecastApplicationCQRSInjector();
        }
    }
}