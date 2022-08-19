using System.Configuration;
using System.Reflection;
using Application;
using Application.Base;
using Application.Context;
using Domain.Configuration;
using Domain.Enums;
using eXtensionSharp;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.BackgroundServices;
using Infrastructure.Persistence;
using Infrastructure.Services.Account;
using Infrastructure.Storage.Files;
using InjectionExtension;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiApplication.Services.Abstract;

namespace Infrastructure
{
    public delegate IStorageProvider StorageProviderResolver(ENUM_STORAGE_TYPE type);
    
    internal class InfrastructureInjector : IServiceInjectionBase
    {
        private readonly Dictionary<ENUM_DATABASE_TYPE,
            Action<string, IServiceProvider, DbContextOptionsBuilder>>
        _useDatabaseStates
            = new()
            {
                {
                    ENUM_DATABASE_TYPE.MSSQL, (connectionString, provider, builder) =>
                    {
                        //MSSQL 
                        builder.UseSqlServer(connectionString, options =>
                            {
                                options.MigrationsAssembly(ApplicationConst.MIGRATION_ASSEMPLY_NAME);
                                //builder.EnableRetryOnFailure();
                                options.CommandTimeout(ApplicationConst.DATABASE_TIMEOUT);
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
                        builder.UseMySql(connectionString, serverVersion, options =>
                            {
                                options.MigrationsAssembly(ApplicationConst.MIGRATION_ASSEMPLY_NAME);
                                //builder.EnableRetryOnFailure();
                                options.CommandTimeout(ApplicationConst.DATABASE_TIMEOUT);
                            })
                            .AddInterceptors(provider.GetRequiredService<DbL4Interceptor>())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                    }
                },
                {
                    ENUM_DATABASE_TYPE.POSTGRES, (connectionString, provider, builder) =>
                    {
                        builder.UseNpgsql(connectionString, options =>
                            {
                                options.MigrationsAssembly(ApplicationConst.MIGRATION_ASSEMPLY_NAME);
                                //builder.EnableRetryOnFailure();
                                options.CommandTimeout(ApplicationConst.DATABASE_TIMEOUT);
                            })
                            .AddInterceptors(provider.GetRequiredService<DbL4Interceptor>())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();                        
                    }
                }
            };
        
        private readonly Dictionary<ENUM_STORAGE_TYPE, Func<IServiceProvider, IStorageProvider>>
            _notifyState =
                new ()
                {
                    { ENUM_STORAGE_TYPE.NCP, (s) => s.GetService<NcpStorage>() },
                    { ENUM_STORAGE_TYPE.LOCAL, (s) => s.GetService<LocalStorage>() },
                };
        
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            #region [database init and seeding]
            //사용할 database 설정
            var useDatabase = configuration.GetConnectionString("USE_DATABASE");
            //enum 으로 변환
            var databaseType = ENUM_DATABASE_TYPE.Parse(useDatabase);
            //연결 문자열 확인 (TODO:연결 문자열 암복호화 되어 있어야 함)
            var connectionString = configuration.GetConnectionString(databaseType.ToString());

            if (databaseType == ENUM_DATABASE_TYPE.MSSQL)
            {
                services.AddHangfire(x => x.UseSqlServerStorage(connectionString));    
            }
            else if (databaseType == ENUM_DATABASE_TYPE.POSTGRES)
            {
                services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));    
            }
            else if (databaseType == ENUM_DATABASE_TYPE.MYSQL)
            {
                throw new NotSupportedException("mysql hangfire not implemented. don't use mysql");
            }
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    var action = _useDatabaseStates[databaseType];
                    action(connectionString, sp, options);
                })
                .AddScoped<IDatabaseSeeder, DatabaseSeeder>()
                .AddScoped<DatabaseMigration>();            
                #endregion
                
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
            
            services.AddScoped<NcpStorage>()
                .AddScoped<LocalStorage>()
                .AddScoped<StorageProviderResolver>(provider => key =>
                {
                    var func = _notifyState[key];
                    if (func.xIsEmpty()) throw new NotImplementedException($"key {key.ToString()} not implemented");
                    return func(provider);
                });
            
            //services.Configure<StorageConfigOption>(configuration.GetSection(nameof(StorageConfigOption)));
            services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(10));
        }
    }

    public static class InfrastructureInjectorExtensions
    {
        public static void AddInfrastructureInjector(this IServiceCollection services, IConfiguration configuration)
        {
            var injectorImpl = new ServiceLoader(new IServiceInjectionBase[]
            {
                new InfrastructureInjector()
            }, services, configuration);
            injectorImpl.Inject();
        }
    }
}