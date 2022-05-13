using System.Linq;
using Application.Context;
using Infrastructure.Abstract;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Controllers;
using WebApiApplication.Extensions;

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

            #region [custom json config]

            services.Configure<FilterOption>(Configuration.GetSection("FilterOption"));

            #endregion

            #region [disable model validation]

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });            

            #endregion

            //services.AddScoped<ISampleRepository, SampleRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, ApplicationDbContext db)
        {
            // db.Database.EnsureCreated();
            // if (db.Database.GetPendingMigrations().Count() > 0)
            // {
            //     db.Database.Migrate();
            // }
            app.UseConfigures(env, provider, Configuration);
        }
    }
}