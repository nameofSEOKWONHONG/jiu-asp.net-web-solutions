using System.Reflection;
using Application.Abstract;
using Infrastructure.BackgroundServices;
using Infrastructure.Services.Account;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services
{
    public class InfrastructureInjector : IServiceInjectionBase
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
                .AddHostedService<ConfigReloadBackgroundService>()
                .AddMediatR(Assembly.Load(nameof(Infrastructure)));

            services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(10));
        }
    }

    public static class InfrastructureInjectorExtensions
    {
        public static void AddInfrastructureInjector(this IServiceCollection services)
        {
            var injectorImpl = new ServiceLoader(new[]
            {
                new InfrastructureInjector()
            }, services, null);
            injectorImpl.Inject();
        }
    }
}