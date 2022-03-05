using System.Xml.Serialization;
using Application.Infrastructure.Injection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Counter;

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton)]
public sealed class CounterView : ViewBase
{
    private int _count;
    public CounterView(ILogger<CounterView> logger) : base(logger)
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
