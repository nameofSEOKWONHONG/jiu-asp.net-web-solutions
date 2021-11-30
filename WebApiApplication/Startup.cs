using System;
using System.IO;
using System.Reflection;
using System.Text;
using Domain.Configuration;
using Hangfire;
using Infrastructure.Context;
using Infrastructure.Middelware;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebApiApplication.Extensions;
using WebApiApplication.Filters;

namespace WebApiApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigureServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            #region [use cors]
            app.UseCors("CorsPolicy");            
            #endregion
            
            app.UseDevelopmentHandling(env);
            app.ConfigureSwagger(env, provider);
            app.UseStaticFiles();
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            //     RequestPath = new PathString("/Files")
            // });
            
            app.UseResponseCache();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomMiddleware();
            app.UseHangfire();

            #region [use blazor hosting same domain]
            //blazor wasm은 정적파일이므로 Api와 같은 호스트(프로젝트)로 배포할 수 있다.
            //물론 별도로 호스트할 수도 있고, 아래는 해당 방법을 설정하는 내용이다.
            //ref : https://stackoverflow.com/questions/61011880/how-can-i-host-asp-net-api-and-blazor-web-assembly-like-an-javascript-spa
            app.UseBlazorFrameworkFiles();
            #endregion

            app.UseEndPoints();
        }
    }
}