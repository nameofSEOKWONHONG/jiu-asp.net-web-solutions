using Application.Abstract;
using Application.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application;

public class ApplicationInjector : IDependencyInjectorBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DbL4Provider>()
            .AddSingleton<DbL4Interceptor>();

        services.AddSingleton<ChloeDbContext>();
        
        services.AddDbContext<JIUDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MSSQL"), builder =>
                {
                    builder.MigrationsAssembly("Application");
                    //builder.EnableRetryOnFailure();
                    builder.CommandTimeout(5);
                })
                .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>());
        });
        //services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        services.AddTransient<IDatabaseSeeder, DatabaseSeederUseChloe>();
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