using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Application;
using Application.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using Application.Script;
using Application.Script.SharpScript;
using Domain.Configuration;
using Hangfire;
using HelloWorldService;
using Infrastructure.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoService;
using WeatherForecastService;

namespace WebApiApplication.Extensions
{
    /// <summary>
    /// ConfigService에 등록될 Add Method들의 집합
    /// </summary>
    internal static class ConfigureServicesExtensions
    {
        /// <summary>
        /// ServiceCollection에 등록, 등록 순서는 영향이 없다.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        internal static void AddConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region [add controllers]
            services.AddControllers();
            #endregion
            
            #region [add httpcontext accessor to the container.]
            services.AddHttpContextAccessor();
            #endregion          
            
            #region [api versioning]
            services.AddVersionConfig();
            #endregion
            
            #region [add auth]
            services.AddAuthentication();
            #endregion
            
            services.AddApplicationInjector(configuration);
            
            AddInjectors(services, configuration);
            AddSwagger(services);
            AddJwt(services, configuration);
            AddCors(services);
            AddResponseCache(services);
            AddHangfire(services, configuration);
            AddBackgroundService(services);
            
            services.AddRazorPages();
            
            services.AddPluginFiles(configuration);
        }

        /// <summary>
        /// 왜 인젝터라는 걸 만들어서 써야 하나?
        /// 보통 닷넷에서 IoC컨테이너는 Autofac을 사용하겠지만 개인적인 사용 경험으로는 Autofac을 사용하여 Assembly로드로 한번에 로드하여
        /// 사용하는 방식으로 대부분 사용할 것이다. 물론 개별로 설정하여 사용할 수 있지만, 대부분의 프로젝트에서 그런 케이스를 보지 못했다.
        /// 따라서 내부 구현에서 transient, soope, singleton을 별도로 구현해서 사용한다.
        /// 이런 구현이 좋은 방향일까... 절대 아니라고 생각한다. IoC컨테이너 설계 철학에도 맞지 않는다.
        /// 따라서, 대부분의 프로젝트를 작은 단위로 유지하면서 아래와 같이 공통으로 처리하는 Injector를 갖는 것이 최선이라고 생각된다.
        /// 물론 Autofac에서도 동일하게 만들수 있지만, dotnet core 3 이상 부터는 IoC컨테이너를 자체적으로 지원하므로 괜히 외부의존성을
        /// 늘릴 필요는 없다고 생각된다.
        /// 일부 사람들은 너무 구식 방식이라고 하던데... 어떻게 해야 최신으로 하는 것인지는 의문이다.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void AddInjectors(IServiceCollection services, IConfiguration configuration)
        {
            // 각 Injector 구현체 등록
            services.AddWeatherForecastInjector();
            services.AddInfrastructureInjector();
            services.AddTodoInjector();
            services.AddHelloWorldInjector();
        }

        private static void AddSwagger(IServiceCollection services)
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
        }

        private static void AddJwt(IServiceCollection services, IConfiguration configuration)
        {
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
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Issuer"],
                        SaveSigninToken = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });


            #endregion            
        }

        private static void AddCors(IServiceCollection services)
        {
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
        }

        private static void AddResponseCache(IServiceCollection services)
        {
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
        }

        private static void AddHangfire(IServiceCollection services, IConfiguration configuration)
        {
            #region [add hangfire]
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("MSSQL")));
            services.AddHangfireServer();
            #endregion
        }

        private static void AddBackgroundService(IServiceCollection services)
        {
            
            #region [add background service]
            #endregion
        }

        private static void AddIdentity(IServiceCollection services)
        {
            // services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            //     .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            //     .AddIdentity<User, Role>(options =>
            //     {
            //         options.Password.RequiredLength = 6;
            //         options.Password.RequireDigit = false;
            //         options.Password.RequireLowercase = false;
            //         options.Password.RequireNonAlphanumeric = false;
            //         options.Password.RequireUppercase = false;
            //         options.User.RequireUniqueEmail = true;
            //     })
            //     .AddEntityFrameworkStores<JIUDbContext>()
            //     .AddDefaultTokenProviders();
        }
    }
}