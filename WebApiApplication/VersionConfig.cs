using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication
{
    public static class VersionConfig {
        public static void AddVersionConfig(this IServiceCollection services) {
            services.AddApiVersioning(opt => {
                // opt.AssumeDefaultVersionWhenUnspecified = true;
                // opt.DefaultApiVersion = ApiVersion.Default;

                // this is going to return all available api versions
                opt.ReportApiVersions = true;

                // // Add Media type versioning
                // opt.ApiVersionReader = ApiVersionReader.Combine(
                //     new HeaderApiVersionReader("x-api-version"),
                //     new MediaTypeApiVersionReader("x-api-version")
                // );
            });
            
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                } );
        }
    }
}