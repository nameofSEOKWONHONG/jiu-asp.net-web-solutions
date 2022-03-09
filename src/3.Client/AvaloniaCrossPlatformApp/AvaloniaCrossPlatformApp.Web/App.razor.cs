using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace AvaloniaCrossPlatformApp.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<AvaloniaCrossPlatformApp.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}