using System.Reflection;
using Application.Abstract;
using Application.Interfaces.HelloWorld;
using HelloWorldApplication.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorldApplication
{
    internal class HelloWorldInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IHelloWorldService, HelloWorldService>();
            services.AddMediatR(Assembly.Load("HelloWorldApplication"));
        }
    }

    public static class HelloWorldInjectorExtension
    {
        public static void AddHelloWorldInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new HelloWorldInjector()
            }, services, null);
        }
    }
}