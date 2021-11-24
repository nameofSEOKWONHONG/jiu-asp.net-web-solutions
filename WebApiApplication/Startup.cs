using System;
using System.IO;
using System.Reflection;
using System.Text;
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
            services.AddRegisterService();
            #endregion

            #region [Add MediatR]
            //ref:https://github.com/jbogard/MediatR
            //MediatR은 지미보가드가 만든 CQRS 패턴을 구현한 프레임워크이다.
            //중계자 패턴으로 불리우는데 Command and Query Responsibility Segregation의 약자로
            //MVVM, IoC와 같이 관심사 분리 기능이다.
            //DDD에서 문자 그대로 Search와 그 이외의 기능으로 분리하는 것으로 DB까지 확장해 보면 
            //Command는 RDBMS, Query는 NOSql로 구성하는 방식까지 확장 할 수 있겠다. (만드는 사람 맘이지만...)
            services.AddRegisterCQRS();

            #endregion
            
            #region [add httpcontext accessor to the container.]
            services.AddHttpContextAccessor();
            #endregion            

            #region [api versioning]

            services.AddVersionConfig();            

            #endregion

            #region [memory cache]

            services.AddMemoryCache();

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

            #region [add background service]
            services.AddHostedService<CacheResetBackgroundService>();
            #endregion

            #region [add hangfire]
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("SqlServer")));
            services.AddHangfireServer();
            #endregion
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            #region [add development env]

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            #endregion
            
            #region [cors]
            app.UseCors("CorsPolicy");            
            #endregion
            
            app.UseStaticFiles();
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            //     RequestPath = new PathString("/Files")
            // });
            #region [caching]
            // allow response caching directives in the API Controllers
            app.UseResponseCaching();            
            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            #region [use middleware]
            //만약 jwt 인증을 한다면 UseAuthentication, UseAuthorization 이전과 이후 배치에 따라 
            //HttpContext.Claims에서 확인할 수 있는 설정이 다르다.
            //따라서 Middleware를 등록할 경우 호출 순서가 중요한다.
            app.UseErrorHandler();
            app.UseRequestCulture();
            app.UseAntiXssMiddleware();
            #endregion

            #region [use hangfire]

            //hangfire의 문제는 DB 부하에 있다. MessageQueue처럼 동작하지만 실제 분산 처리가 아닌 스케줄러에 가깝다.
            //위 내용 자체가 틀린지도 모르지만 확실히 스케줄러다...
            //일부는 Database 이슈가 있는 것처럼 보인다... 기본 SqlServer, pro에서 MSMQ 및 Redis를 지원한다.
            //TODO : kafka로도 구축해 보자.
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = "WebApiApplication Jobs",
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            #endregion

            #region [use blazor hosting same domain]

            //blazor wasm은 정적파일이므로 Api와 같은 호스트(프로젝트)로 배포할 수 있다.
            //물론 별도로 호스트할 수도 있고, 아래는 해당 방법을 설정하는 내용이다.
            //ref : https://stackoverflow.com/questions/61011880/how-can-i-host-asp-net-api-and-blazor-web-assembly-like-an-javascript-spa
            app.UseBlazorFrameworkFiles();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToFile("index.html");
                //endpoints.MapControllers();
            });

            #endregion
        }
    }
}