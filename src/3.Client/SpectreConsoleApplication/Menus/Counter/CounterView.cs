using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Counter;

public sealed class CounterView : ViewBase
{
    private int _count;
    public CounterView(ILogger<CounterView> logger) : base(logger)
    {
        
    }
    
    public override void Show()
    {
        CONTINUE:
        var input = AnsiConsole.Ask<int>("try enter counter number (if enter 0 number, is exit) : ");
        if (input > 0)
        {
            AnsiConsole.MarkupLine($"[blue]{_count += input}[/]");
            goto CONTINUE;
        }
    }
}
