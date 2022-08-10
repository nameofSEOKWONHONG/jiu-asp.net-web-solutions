#define __USE_HANGFIRE_SQL_SERVER__

using System.IO;
using Application.Context;
using Hangfire;
using Infrastructure.Middleware;
using Infrastructure.Middlewares;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebApiApplication.Filters;

namespace WebApiApplication.Extensions
{
    internal static class ApplicationBuilderExtenions
    {
        internal static IApplicationBuilder UseConfigures(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            #region [use cors]
            app.UseCors("CorsPolicy");            
            #endregion
            
            app.UseDevelopmentHandling(env);
            app.UseConfigureSwagger(env, provider);
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });

            #region [http logging]

            app.UseHttpLogging();
            app.Use(async (context, next) =>
            {
                context.Response.Headers["ApplicationResponseHeader"] =
                    new string[] {"Application Response Header Value"};
                await next();
            });

            #endregion
            
            app.UseResponseCache();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseConfigureMiddleware();
            app.UseHangfire();

            #region [use blazor hosting same domain]
            //blazor wasm은 정적파일이므로 Api와 같은 호스트(프로젝트)로 배포할 수 있다.
            //물론 별도로 호스트할 수도 있고, 아래는 해당 방법을 설정하는 내용이다.
            //ref : https://stackoverflow.com/questions/61011880/how-can-i-host-asp-net-api-and-blazor-web-assembly-like-an-javascript-spa
            app.UseBlazorFrameworkFiles();
            #endregion

            app.UseConfigureEndPoints();

            app.Initialize(configuration);
            
            return app;
        }
        
        /// <summary>
        /// 개발설정
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseDevelopmentHandling(this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            return app;
        }

        /// <summary>
        /// swagger 설정
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseConfigureSwagger(this IApplicationBuilder app,
            IWebHostEnvironment env, 
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(options => {
                    options.DocExpansion(DocExpansion.None);
                    // build a swagger endpoint for each discovered API version  
                    foreach (var description in provider.ApiVersionDescriptions)  
                    {  
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());                        
                    }  
                    options.RoutePrefix = "swagger";                
                });                
            }

            return app;
        }

        /// <summary>
        /// 응답 캐시 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseResponseCache(this IApplicationBuilder app)
        {
            // allow response caching directives in the API Controllers
            app.UseResponseCaching();    
            // 캐시 미들웨어 등록
            // app.Use(async (context, next) =>
            // {
            //     context.Response.GetTypedHeaders().CacheControl = 
            //         new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            //         {
            //             Public = true,
            //             MaxAge = TimeSpan.FromSeconds(10)
            //         };
            //     context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = 
            //         new string[] { "Accept-Encoding" };
            //
            //     await next();
            // });            
            return app;
        }
        
        /// <summary>
        /// endpoint 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseConfigureEndPoints(this IApplicationBuilder app) => app.UseEndpoints(endpoints =>
        {
            app.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapRazorPages();
                endpointRouteBuilder.MapDefaultControllerRoute();
                endpointRouteBuilder.MapFallbackToFile("index.html");
                //endpoints.MapControllers();
            });
        });

        /// <summary>
        /// 미들웨어 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseConfigureMiddleware(this IApplicationBuilder app)
        {
            //만약 jwt 인증을 한다면 UseAuthentication, UseAuthorization 이전과 이후 배치에 따라 
            //HttpContext.Claims에서 확인할 수 있는 설정이 다르다.
            //따라서 Middleware를 등록할 경우 호출 순서가 중요하다.
            // 예외설정
            app.UseErrorHandler();
            // 문화권 설정
            app.UseRequestLocalizationByCulture();
            // XSS 방어 설정
            app.UseAntiXssMiddleware();
            // custom header middleware
            app.UseMiddleware<CustomHeaderMiddleware>();
            return app;
        }

        /// <summary>
        /// hangfire 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            //hangfire의 문제는 DB 부하에 있다. MessageQueue처럼 동작하지만 실제 분산 처리가 아닌 스케줄러에 가깝다.
            //위 내용 자체가 틀린지도 모르지만 확실히 스케줄러다...
            //일부는 Database 이슈가 있는 것처럼 보인다... 기본 SqlServer, pro에서 MSMQ 및 Redis를 지원한다.
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = "WebApiApplication Jobs",
                Authorization = new[] { new HangfireAuthorizationFilter() },
            });
            
            return app;
        }

        private static IApplicationBuilder Initialize(this IApplicationBuilder app, IConfiguration configuration)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
    }
    
}