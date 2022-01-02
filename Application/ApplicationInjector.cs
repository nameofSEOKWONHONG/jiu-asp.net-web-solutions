using Application.Abstract;
using Application.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public class ApplicationInjector : IDependencyInjectorBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DbL4Provider>()
            .AddSingleton<DbL4Interceptor>();
        
        services.AddDbContext<JIUDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"), builder =>
                {
                    builder.MigrationsAssembly("Application");
                    //builder.EnableRetryOnFailure();
                    builder.CommandTimeout(5);
                })
                .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>());
        }).AddTransient<IDatabaseSeeder, DatabaseSeeder>();
    }
}

public static class ApplicationInjectorExtension
{
    public static void AddApplicationInjector(this IServiceCollection services, IConfiguration configuration)
    {
        var injectorImpl = new DependencyInjectorImpl(new[]
        {
            new ApplicationInjector()
        }, services, configuration);
        injectorImpl.Inject();
    }
}