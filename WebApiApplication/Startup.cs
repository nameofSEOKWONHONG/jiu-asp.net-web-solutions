using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApiApplication.Controllers;
using WebApiApplication.DataContext;
using WebApiApplication.Entities;
using WebApiApplication.Infrastructure;
using WebApiApplication.Services;

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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.ToString());
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApiApplication", Version = "v1"});
                // To Enable authorization using Swagger (JWT)    
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",  
                });  
                c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "Bearer"  
                            }  
                        },  
                        new string[] {}  
  
                    }  
                });  
            });

            #region [service]

            //if use userservice manual implement
            //services.AddSingleton<IUserService, UserService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IWeatherForcastService, WeatherForcastService>();
            
            services.AddSingleton<DbL4Provider>();
            services.AddSingleton<DbL4Interceptor>();
            
            services.AddTransient<BoatCreatorService>();
            services.AddTransient<CarCreatorService>();
            services.AddTransient<BusCreatorService>();
            services.AddSingleton<VehicleCreatorServiceFactory>();

            services.AddTransient<SMSMessageService>();
            services.AddTransient<EmailMessageService>();
            services.AddTransient<KakaoMessageService>();
            services.AddSingleton<MessageServiceFactory>();

            #endregion

            #region [memory cache]

            services.AddMemoryCache();

            #endregion

            #region [database]

            services.AddDbContext<AccountDbContext>((sp, options) =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), builder =>
                        {
                            builder.EnableRetryOnFailure();
                            builder.CommandTimeout(5);
                        })
                        .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>());
                });
            #endregion

            #region [add jwt]

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });


            #endregion
            
            #region [add oauth]

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGitHub(options =>
            {
                options.ClientId = "";
                options.ClientSecret = "";
            }).AddKakaoTalk(options =>
            {
                options.ClientId = "6167cd42b6b2599d8a9d981389224cbc";
                options.ClientSecret = "wATqhEdFme0CX6d3lR2g2hUhPRQ84DiF";
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiApplication v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}