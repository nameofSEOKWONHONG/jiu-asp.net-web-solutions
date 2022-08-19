using System;
using System.Collections.Generic;
using System.IO;
using Application.Base;
using Application.Context;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Configuration;
using Application.Infrastructure.Message;
using Application.Script;
using Application.Script.ClearScript;
using Application.Script.SharpScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Domain.Configuration;
using Domain.Enums;
using Hangfire;
using Hangfire.PostgreSql;
using InjectionExtension;
using Jering.Javascript.NodeJS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public class ApplicationInjector : IServiceInjectionBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer();

        #region [javascript.nodejs]

        services.AddNodeJS()
            .Configure<NodeJSProcessOptions>(options =>
            {
                //현재 실행되는 코드 또는 파일의 프로젝트 패스를 설정한다.
                //module호출을 위해 설정하게 됨.
                options.ProjectPath = Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\node");

                //node 실행경로 설정, 기본 경로가 아닌 경로에서 실행할 경우 아래의 경로를 설정한다.
                //options.ExecutablePath = Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\node\\bin");
            })
#if NODE_DEBUG
            .Configure<NodeJSProcessOptions>(options => options.NodeAndV8Options = "--inspect-brk")
            .Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = -1)
            .Configure<HttpNodeJSServiceOptions>(options => options.Version = HttpVersion.Version20)
#endif
            ;        

        #endregion

        services.Configure<MongoDbOption>(configuration.GetSection(nameof(MongoDbOption)));

        services

            #region [db l4 provider - table name replace to template table name]

            .AddSingleton<DbL4Provider>()
            .AddSingleton<DbL4Interceptor>()

            #endregion

            #region [define script loader (cs, js, python, node)]

            .AddSingleton<ScriptInitializer>()
            .AddSingleton<SharpScriptLoader>()
            .AddSingleton<JsScriptLoader>()
            .AddSingleton<PyScriptLoader>()
            .AddSingleton<JIntScriptLoader>()
            .AddSingleton<NodeJSScriptLoader>();

            #endregion
            
        //services.AddTransient<IDatabaseSeeder, DatabaseSeederUseChloe>();

        #region [config 설정]
        
        services.Configure<ScriptLoaderConfig>(configuration.GetSection(nameof(ScriptLoaderConfig)));
        services.Configure<MailSetting>(configuration.GetSection(nameof(MailSetting)));
        services.Configure<CacheConfig>(configuration.GetSection(nameof(CacheConfig)));
        services.Configure<FileFilterOptions>(configuration.GetSection(nameof(FileFilterOptions)));

        services.AddScoped<FileFilterSetting>();
        
        #region [mongodb 설정]
        services.Configure<MongoDbOption>(configuration.GetSection(nameof(MongoDbOption)));
        #endregion
        
        #endregion

        #region [quartz 설정]

        // services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));
        // //or
        // // if you are using persistent job store, you might want to alter some options
        // // services.Configure<QuartzOptions>(options =>
        // // {
        // //     options.Scheduling.IgnoreDuplicates = true; // default: false
        // //     options.Scheduling.OverWriteExistingData = true; // default: true
        // // });
        //
        // //setting reference : https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#di-aware-job-factories
        // services.AddQuartz(config =>
        // {
        //     // handy when part of cluster or you want to otherwise identify multiple schedulers
        //     config.SchedulerId = "Scheduler-Core";
        //
        //     // we take this from appsettings.json, just show it's possible
        //     // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";
        //
        //     // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
        //     config.UseMicrosoftDependencyInjectionJobFactory();
        //     
        //     //default
        //     // config.UseSimpleTypeLoader();
        //     // config.UseInMemoryStore();
        //     config.UseDefaultThreadPool(m => m.MaxConcurrency = 10);
        //     // this is the default 
        //     // config.WithMisfireThreshold(TimeSpan.FromSeconds(60)
        //     config.UsePersistentStore(x =>
        //     {
        //         // force job data map values to be considered as strings
        //         // prevents nasty surprises if object is accidentally serialized and then 
        //         // serialization format breaks, defaults to false
        //         x.UseProperties = true;
        //         x.UseClustering();
        //         // there are other SQL providers supported too 
        //         x.UseSqlServer(connectionString);
        //         // this requires Quartz.Serialization.Json NuGet package
        //         x.UseJsonSerializer();
        //     });
        //     // // job initialization plugin handles our xml reading, without it defaults are used
        //     // // requires Quartz.Plugins NuGet package
        //     // config.UseXmlSchedulingConfiguration(x =>
        //     // {
        //     //     x.Files = new[] { "~/quartz_jobs.xml" };
        //     //     // this is the default
        //     //     x.FailOnFileNotFound = true;
        //     //     // this is not the default
        //     //     x.FailOnSchedulingError = true;
        //     // })
        //     // .BuildScheduler();
        // });
        //
        // services.AddQuartzServer(config =>
        // {
        //     config.WaitForJobsToComplete = true;
        // });

        #endregion
    }
}

public static class ApplicationInjectorExtension
{
    public static void AddApplicationInjector(this IServiceCollection services, IConfiguration configuration)
    {
        var injectorImpl = new ServiceLoader(new IServiceInjectionBase[]
        {
            new ApplicationInjector(),
            new NotifyMessageProviderInjector(),
            new CacheProviderInjector(),
        }, services, configuration);
        injectorImpl.Inject();
    }
}