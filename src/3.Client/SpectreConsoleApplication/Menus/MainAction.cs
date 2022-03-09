using Domain.Base;
using InjectionExtension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Menus.Counter;
using SpectreConsoleApplication.Menus.Member;
using SpectreConsoleApplication.Menus.WeatherForecast;

namespace SpectreConsoleApplication.Menus;

public interface IMainAction
{
    IEnumerable<string> GetMenuNames();
    ViewBaseCore GetView(string menuName, IServiceProvider serviceProvider);
}

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton, typeof(IMainAction))]
public class MainAction : ActionBase, IMainAction
{
    private readonly Dictionary<string, Func<IServiceProvider, ViewBaseCore>> _menuViewStates = new() 
    {
        {"Counter", (s) => new ViewBaseCore(s.GetRequiredService<CounterView>())},
        {"WeatherForecast", (s) => new ViewBaseCore(s.GetRequiredService<WeatherForecastView>())},
        {"Member", (s) => new ViewBaseCore(s.GetRequiredService<MemberView>())},
        {"Exit", (s) => null}
    };
    
    public MainAction(ILogger<MainView> logger, IClientSession clientSession, IHttpClientFactory clientFactory) : base(logger, clientSession, clientFactory)
    {
    }

    public IEnumerable<string> GetMenuNames()
    {
        return _menuViewStates.Keys;
    }

    public ViewBaseCore GetView(string menuName, IServiceProvider serviceProvider)
    {
        return _menuViewStates[menuName](serviceProvider);
    }
}