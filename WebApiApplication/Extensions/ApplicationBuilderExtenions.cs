using Hangfire;
using Infrastructure.Middelware;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebApiApplication.Filters;

namespace WebApiApplication.Extensions
{
    internal static class ApplicationBuilderExtenions
    {
        /// <summary>
        /// 개발설정
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseDevelopmentHandling(this IApplicationBuilder app,
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
        internal static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app,
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
        internal static IApplicationBuilder UseResponseCache(this IApplicationBuilder app)
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
        internal static IApplicationBuilder UseEndPoints(this IApplicationBuilder app) => app.UseEndpoints(endpoints =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToFile("index.html");
                //endpoints.MapControllers();
            });
        });

        /// <summary>
        /// 미들웨어 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            //만약 jwt 인증을 한다면 UseAuthentication, UseAuthorization 이전과 이후 배치에 따라 
            //HttpContext.Claims에서 확인할 수 있는 설정이 다르다.
            //따라서 Middleware를 등록할 경우 호출 순서가 중요한다.
            // 예외설정
            app.UseErrorHandler();
            // 문화권 설정
            app.UseRequestCulture();
            // XSS 방어 설정
            app.UseAntiXssMiddleware();
            return app;
        }

        /// <summary>
        /// hangfire 설정
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            //hangfire의 문제는 DB 부하에 있다. MessageQueue처럼 동작하지만 실제 분산 처리가 아닌 스케줄러에 가깝다.
            //위 내용 자체가 틀린지도 모르지만 확실히 스케줄러다...
            //일부는 Database 이슈가 있는 것처럼 보인다... 기본 SqlServer, pro에서 MSMQ 및 Redis를 지원한다.
            //TODO : kafka로도 구축해 보자.
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = "WebApiApplication Jobs",
                Authorization = new[] { new HangfireAuthorizationFilter() },
            });

            return app;
        }
        
    }
    
}