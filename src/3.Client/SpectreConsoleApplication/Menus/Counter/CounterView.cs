using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Counter;

public sealed class CounterView : MenuViewBase
{
    private int _count;
    public CounterView(ILogger<CounterView> logger) : base(logger)
    {
        
    }
    
    public override void Show()
    {
        while (true)
        {
            var input = AnsiConsole.Ask<int>("try enter counter number (if enter 0 number, is exist) : ");
            if(input <= 0) return;
            AnsiConsole.MarkupLine($"[blue]{_count += input}[/]");
        }
    }
}