using Application.Abstract;
using Infrastructure.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services
{
    public class InfrastructureInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISessionContextService, SessionContextService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}