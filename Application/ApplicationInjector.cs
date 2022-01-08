using System;
using Application.Abstract;
using Application.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DbContextBase = Application.Context.DbContextBase;
namespace Application;

public class ApplicationInjector : IDependencyInjectorBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DbL4Provider>()
            .AddSingleton<DbL4Interceptor>();
        
        services.AddDbContext<JIUDbContext>((sp, options) =>
        {
            //사용할 database 설정
            var useDatabase = configuration.GetConnectionString("USE_DATABASE");
            //enum 으로 변환
            var databaseType = DbContextBase.ENUM_DATABASE_TYPE.Parse(useDatabase);
            //연결 문자열 확인 (TODO:연결 문자열 암복호화 되어 있어야 함)
            var connectionString = configuration.GetConnectionString(databaseType.ToString());
            //사용할 db에 따라 구분
            if (databaseType == DbContextBase.ENUM_DATABASE_TYPE.MSSQL)
            {
                //MSSQL 
                options.UseSqlServer(connectionString, builder =>
                    {
                        builder.MigrationsAssembly("Application");
                        //builder.EnableRetryOnFailure();
                        builder.CommandTimeout(5);
                    })
                    .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>())
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
            else if (databaseType == DbContextBase.ENUM_DATABASE_TYPE.MYSQL)
            {
                //MYSQL
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
                options.UseMySql(connectionString, serverVersion, builder =>
                    {
                        builder.MigrationsAssembly("Application");
                        //builder.EnableRetryOnFailure();
                        builder.CommandTimeout(5);
                    })
                    .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>())
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
            else if (databaseType == DbContextBase.ENUM_DATABASE_TYPE.POSTGRES)
            {
                //POSTGRESQL
                options.UseNpgsql(connectionString, builder =>
                    {
                        builder.MigrationsAssembly("Application");
                        //builder.EnableRetryOnFailure();
                        builder.CommandTimeout(5);
                    })
                    .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>())
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });
        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        //services.AddTransient<IDatabaseSeeder, DatabaseSeederUseChloe>();
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