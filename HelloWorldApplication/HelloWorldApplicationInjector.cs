using System.Reflection;
using Application.Abstract;
using Application.Interfaces.HelloWorld;
using HelloWorldApplication.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorldApplication
{
    public class HelloWorldApplicationInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IHelloWorldService, HelloWorldService>();
        }
    }
    
    public class HelloWorldApplicationCQRSInjector : CQRSInjectorBase
    {
        public override Assembly GetAssembly()
        {
            return Assembly.Load("HelloWorldApplication");
        }
    }

    public static class HelloWorldApplicationInjectorExtension
    {
        public static void AddHelloWorldApplicationInjector(this IServiceCollection services)
        {
            var injector = new HelloWorldApplicationInjector();
            injector.Inject(services, null);
        }

        public static void AddHelloWorldApplicationCQRSInjector(this IServiceCollection services)
        {
            var injector = new HelloWorldApplicationCQRSInjector();
            services.AddMediatR(injector.GetAssembly());
        }
    }
}