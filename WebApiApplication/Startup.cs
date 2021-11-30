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
            #region [swagger setting]
            services.AddSwaggerGen(options =>
            {
                //do not display schema
                //options.DocumentFilter<SwaggerRemoveSchemasFilter>();

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
            services.AddConfigureServices(Configuration);
            #endregion
            
            #region [add httpcontext accessor to the container.]
            services.AddHttpContextAccessor();
            #endregion            

            #region [api versioning]

            services.AddVersionConfig();            

            #endregion

            #region [add database]
            services.AddDbContext<JUIDbContext>((sp, options) =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), builder =>
                        {
                            builder.MigrationsAssembly("Infrastructure");
                            builder.EnableRetryOnFailure();
                            builder.CommandTimeout(5);
                        })
                        .AddInterceptors(sp.GetRequiredService<DbL4Interceptor>());
                });
            #endregion

            #region [add jwt]

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        SaveSigninToken = true,
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
            services.AddResponseCaching(options =>
            {
                // //MaximumBodySize 같거나 작은 크기의 응답을 캐시
                // options.MaximumBodySize = 1024;
                // //경로 대소문자 구분하여 캐시
                // options.UseCaseSensitivePaths = true;
            });
            //.AddNewtonsoftJson(o => o.SerializerSettings.Converters.Insert(0, new CustomConverter()));            
            #endregion

            #region [add background service]
            services.AddHostedService<CacheResetBackgroundService>();
            #endregion

            #region [add hangfire]
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("SqlServer")));
            services.AddHangfireServer();
            #endregion
            
            #region [add config]
            services.Configure<EMailSettings>(Configuration.GetSection("EMailSettings"));
            #endregion
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            #region [use cors]
            app.UseCors("CorsPolicy");            
            #endregion
            
            app.UseExceptionHandling(env);
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