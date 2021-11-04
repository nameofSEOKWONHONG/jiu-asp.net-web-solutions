using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApiApplication.Controllers;
using WebApiApplication.DataContext;
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
            #region [swagger setting]
            services.AddSwaggerGen(options =>
            {
                //do not display schema
                options.DocumentFilter<SwaggerRemoveSchemasFilter>();

                //disable realem object schema error
                options.CustomSchemaIds(type => type.ToString());

                //remove swagger api version error
                options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));

                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments into swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                
                // add a custom operation filter which sets default values
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                    });

                // add a custom operation filter which sets default values
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });                
            });            
            #endregion

            #region [add controllers]
            services.AddControllers();        
            #endregion

            #region [add service]
            
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
            // add factory for message service
            services.AddSingleton<MessageServiceFactory>();

            services.AddSingleton<IGenerateViewService, GenerateViewService>();

            #endregion

            #region [api versioning]

            services.AddVersionConfig();            

            #endregion

            #region [memory cache]

            services.AddMemoryCache();

            #endregion

            #region [add database]

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
            
            #region [add auth]

            services.AddAuthentication();

            #endregion

            #region [add cors]
            // USE CORS
            // ref : https://stackoverflow.com/questions/53675850/how-to-fix-the-cors-protocol-does-not-allow-specifying-a-wildcard-any-origin
            // ********************
            services.AddCors(options => {
                //options.AddPolicy("AllowAll",
                //    builder => {
                //        builder
                //        .AllowAnyOrigin()
                //        .AllowAnyMethod()
                //        .AllowAnyHeader()
                //        .AllowCredentials();
                //    });
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });            
            #endregion      

            #region [add response caching]      
            // add service for allowing caching of responses
            // ref : https://github.com/Cingulara/dotnet-core-web-api-caching-examples
            services.AddResponseCaching();
            //.AddNewtonsoftJson(o => o.SerializerSettings.Converters.Insert(0, new CustomConverter()));            
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(options => {                                
                    // build a swagger endpoint for each discovered API version  
                    foreach (var description in provider.ApiVersionDescriptions)  
                    {  
                        options.DefaultModelExpandDepth(-1);
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());                        
                    }  
                    options.RoutePrefix = string.Empty;                
                });
            }

            #region [cors]
            app.UseCors("CorsPolicy");            
            #endregion

            #region [caching]
            // allow response caching directives in the API Controllers
            app.UseResponseCaching();            
            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}