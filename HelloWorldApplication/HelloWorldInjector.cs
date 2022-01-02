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
            var @namespace = this.GetType().Namespace;
            services.AddScoped<IHelloWorldService, HelloWorldService>()
                .AddMediatR(Assembly.Load(@namespace));
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