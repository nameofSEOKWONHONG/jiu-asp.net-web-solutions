using System;
using Application.Abstract;
using JUIControls.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JUIControls;

public delegate IFormMaker FormProviderResolver(string manuCode);

internal class FormProviderInjector : IDependencyInjectorBase
{
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DashBoardForm>()
            .AddSingleton<FormProviderResolver>(provider => key =>
            {
                if (key == "DashBoard") return provider.GetService<DashBoardForm>();
                else throw new NotImplementedException();
            });
    }
}

public static class FormProviderInjectorExtension
{
    public static void AddSectionProviderInjector(this IServiceCollection services)
    {
        var impl = new DependencyInjector(new[]
        {
            new FormProviderInjector()
        }, services, null);
        impl.Inject();
    }
}