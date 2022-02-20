using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SpectreConsoleApplication.Menus.Counter;

public sealed class CounterView : MenuViewBaseBase
{
    public CounterView(ILogger<CounterView> logger) : base(logger)
    {
        
    }
    
    public override void Show()
    {
        int count = 0;
        while (true)
        {
            var input = AnsiConsole.Ask<int>("try enter counter number (if enter 0 number, is exist) : ");
            if(input <= 0) return;
            AnsiConsole.MarkupLine($"[blue]{count += input}[/]");
        }
    }
}