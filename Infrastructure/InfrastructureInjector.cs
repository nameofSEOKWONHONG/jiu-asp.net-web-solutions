using System.Reflection;
using Application.Abstract;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Services.Identity;
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
            services.AddSingleton<DbL4Provider>()
                .AddSingleton<DbL4Interceptor>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IRoleClaimService, RoleClaimService>()
                .AddScoped<ISessionContextService, SessionContextService>()
                .AddScoped<IUserService, UserService>()
                .AddMediatR(Assembly.Load("Infrastructure"));
        }
    }

    public static class InfrastructureInjectorExtensions
    {
        public static void AddInfrastructureInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new InfrastructureInjector()
            }, services, null);
            diCore.Inject();
        }
    }
}