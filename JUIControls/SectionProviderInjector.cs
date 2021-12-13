using System;
using Application.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JUIControls;

public delegate ISectionMaker SectionProviderResolver(string manuCode);

internal class SectionProviderInjector : IDependencyInjectorBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DashBoardSection>()
            .AddSingleton<SectionProviderResolver>(provider => key =>
            {
                if (key == "DashBoard") return provider.GetService<DashBoardSection>();
                else throw new NotImplementedException();
            });
    }
}

public static class SectionProviderInjectorExtension
{
    public static void AddSectionProviderInjector(this IServiceCollection services)
    {
        var impl = new DependencyInjectorImpl(new[]
        {
            new SectionProviderInjector()
        }, services, null);
        impl.Inject();
    }
}