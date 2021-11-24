using System.Reflection;
using Application.Abstract;
using Infrastructure.Context;
using Infrastructure.Services.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services
{
    public class InfrastructureInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddSingleton<DbL4Provider>();
            services.AddSingleton<DbL4Interceptor>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISessionContextService, SessionContextService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
    
    public class InfrastructureCQRSInjector : CQRSInjectorBase
    {
        public override Assembly GetAssembly()
        {
            return Assembly.Load("Infrastructure");
        }
    }

    public static class InfrastructureInjectorExtensions
    {
        public static void AddInfrastructureInjector(this IServiceCollection services)
        {
            var injector = new InfrastructureInjector();
            injector.Inject(services);
        }

        public static void AddInfrastructureCQRSInjector(this IServiceCollection services)
        {
            var injector = new InfrastructureCQRSInjector();
            services.AddMediatR(injector.GetAssembly());
        }
    }
}