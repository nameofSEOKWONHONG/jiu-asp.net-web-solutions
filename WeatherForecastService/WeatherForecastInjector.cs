using System.Reflection;
using Application.Abstract;
using Application.Interfaces.WeahterForecast;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecastService.Services;

namespace WeatherForecastService
{
    internal class WeatherForecastInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWeatherForcastService, WeatherForcastService>();
            
            /// MediatR은 지미보가드가 만든 CQRS 패턴을 구현한 프레임워크이다.
            /// 중계자 패턴으로 불리우는데 Command and Query Responsibility Segregation의 약자로
            /// MVVM, IoC와 같이 관심사 분리 기능이다.
            /// DDD에서 문자 그대로 Search와 그 이외의 기능으로 분리하는 것으로 DB까지 확장해 보면 
            /// Command는 RDBMS, Query는 NOSql로 구성하는 방식까지 확장 할 수 있겠다. (만드는 사람 맘이지만...)
            /// <see cref="https://github.com/jbogard/MediatR"/>
            services.AddMediatR(Assembly.Load("WeatherForecastService"));
        }
    }

    public static class WeatherForecastInjectorExtension
    {
        public static void AddWeatherForecastInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectionLoader(new[]
            {
                new WeatherForecastInjector()
            }, services, null);
            diCore.Inject();
        }
    } 
}