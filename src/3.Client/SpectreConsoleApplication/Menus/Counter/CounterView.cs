using Domain.Base;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Counter;

[AddService(ENUM_LIFE_TIME_TYPE.Singleton)]
public sealed class CounterView : ViewBase
{
    private int _count;
    public CounterView(ILogger<CounterView> logger,
        IClientSession clientSession) : base(logger, clientSession)
    {
        
    }
    
    public override void Show()
    {
        var input = AnsiConsole.Ask<int>("try enter counter number : ");
        if (input > 0)
        {
            AnsiConsole.MarkupLine($"[blue]{_count += input}[/]");
        }
    }
}
