using System.Reflection;
using Application.Base;
using Application.Interfaces.HelloWorld;
using eXtensionSharp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorldService
{
    internal class HelloWorldInjector : IServiceInjectionBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            var @namespace = this.GetType().Namespace.xValue();
            services.AddScoped<IHelloWorldService, Services.HelloWorldService>()
                .AddMediatR(Assembly.Load(@namespace));
        }
    }

    public static class HelloWorldInjectorExtension
    {
        public static void AddHelloWorldInjector(this IServiceCollection services)
        {
            var diCore = new ServiceLoader(new IServiceInjectionBase[]
            {
                new HelloWorldInjector()
            }, services, null);
            diCore.Inject();
        }
    }
}