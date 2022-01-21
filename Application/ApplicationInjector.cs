using System;
using System.Collections.Generic;
using Application.Abstract;
using Application.Context;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using Application.Script;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Domain.Configuration;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public class ApplicationInjector : IDependencyInjectorBase
{
    private readonly Dictionary<ENUM_DATABASE_TYPE,
            Action<string, IServiceProvider, DbContextOptionsBuilder>>
        _useDatabaseState
            = new Dictionary<ENUM_DATABASE_TYPE,
                Action<string, IServiceProvider, DbContextOptionsBuilder>>()
            {
                {
                    ENUM_DATABASE_TYPE.MSSQL, (connectionString, provider, builder) =>
                    {
                        //MSSQL 
                        builder.UseSqlServer(connectionString, builder =>
                            {
                                builder.MigrationsAssembly("Application");
                                //builder.EnableRetryOnFailure();
                                builder.CommandTimeout(5);
                            })
                            .AddInterceptors(provider.GetRequiredService<DbL4Interceptor>())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                    }
                },
                {
                    ENUM_DATABASE_TYPE.MYSQL, (connectionString, provider, builder) =>
                    {
                        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
                        builder.UseMySql(connectionString, serverVersion, builder =>
                            {
                                builder.MigrationsAssembly("Application");
                                //builder.EnableRetryOnFailure();
                                builder.CommandTimeout(5);
                            })
                            .AddInterceptors(provider.GetRequiredService<DbL4Interceptor>())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                    }
                },
                {
                    ENUM_DATABASE_TYPE.POSTGRES, (connectionString, provider, builder) =>
                    {
                        builder.UseNpgsql(connectionString, builder =>
                            {
                                builder.MigrationsAssembly("Application");
                                //builder.EnableRetryOnFailure();
                                builder.CommandTimeout(5);
                            })
                            .AddInterceptors(provider.GetRequiredService<DbL4Interceptor>())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();                        
                    }
                }
            };
    
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DbL4Provider>()
            .AddSingleton<DbL4Interceptor>()
            .AddSingleton<SharpScriptLoader>()
            .AddSingleton<JsScriptLoader>()
            .AddDbContext<JIUDbContext>((sp, options) =>
            {
                //사용할 database 설정
                var useDatabase = configuration.GetConnectionString("USE_DATABASE");

                //enum 으로 변환
                var databaseType = ENUM_DATABASE_TYPE.Parse(useDatabase);

                //연결 문자열 확인 (TODO:연결 문자열 암복호화 되어 있어야 함)
                var connectionString = configuration.GetConnectionString(databaseType.ToString());

                var action = _useDatabaseState[databaseType];
                action(connectionString, sp, options);
            })
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        //services.AddTransient<IDatabaseSeeder, DatabaseSeederUseChloe>();
        services.Configure<ScriptLoaderConfig>(configuration.GetSection(nameof(ScriptLoaderConfig)));
        services.Configure<EMailSettings>(configuration.GetSection(nameof(EMailSettings)));
    }
}

public static class ApplicationInjectorExtension
{
    public static void AddApplicationInjector(this IServiceCollection services, IConfiguration configuration)
    {
        var injectorImpl = new DependencyInjector(new IDependencyInjectorBase[]
        {
            new ApplicationInjector(),
            new NotifyMessageProviderInjector(),
            new CacheProviderInjector(),
        }, services, configuration);
        injectorImpl.Inject();
    }
}