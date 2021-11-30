﻿using System.Reflection;
using Application.Abstract;
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
            services.AddSingleton<DbL4Provider>();
            services.AddSingleton<DbL4Interceptor>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISessionContextService, SessionContextService>();
            services.AddScoped<IUserService, UserService>();
            services.AddMediatR(Assembly.Load("Infrastructure"));
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