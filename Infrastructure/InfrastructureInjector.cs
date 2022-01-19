using System.Reflection;
using Application.Abstract;
using Infrastructure.Services.Account;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services
{
    public class InfrastructureInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IRolePermissionService, RolePermissionService>()
                .AddScoped<ISessionContextService, SessionContextService>()
                .AddHostedService<CacheResetBackgroundService>()
                .AddHostedService<HardwareMonitorBackgroundService>()
                .AddMediatR(Assembly.Load("Infrastructure"));
        }
    }

    public static class InfrastructureInjectorExtensions
    {
        public static void AddInfrastructureInjector(this IServiceCollection services)
        {
            var injectorImpl = new DependencyInjector(new[]
            {
                new InfrastructureInjector()
            }, services, null);
            injectorImpl.Inject();
        }
    }
}