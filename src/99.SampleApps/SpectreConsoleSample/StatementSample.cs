using eXtensionSharp;
using Spectre.Console;

namespace SpectreConsoleSample;

public class SpectreConsoleStatementSample
{
    public SpectreConsoleStatementSample()
    {
        
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Hello, World!");
        AnsiConsole.MarkupLine("[red]Hello World[/]");
        //"[/]" means is markup end
        AnsiConsole.MarkupLine("[underline red]Hello[/] World!");

        // markup formatting
        AnsiConsole.MarkupLine("[red]{0}[/]", "Hello [World]".EscapeMarkup());
        //or
        AnsiConsole.MarkupLine("[red]{0}[/]", Markup.Escape("Hello [World]"));

        //set background color
        //[bold red(foreign color) on blue(background color) ]
        AnsiConsole.MarkupLine("[bold red on blue]Hello[/]");
        AnsiConsole.MarkupLine("[default on blue]World[/]");

        //render emojis
        AnsiConsole.MarkupLine($"hello {Emoji.Known.Alien}!");

        //exception
        Exception e = new NotImplementedException("test exception");
        AnsiConsole.WriteException(e);

        #region [live display]

        var style = new Style(Color.Orange1, Color.Aqua, Decoration.Bold, "https://bing.com");
        var markup = new Markup("Live Display Sample", style)
        {
            Alignment = Justify.Center
        };
        AnsiConsole.Write(markup);

        var table = new Table().Centered();
        var columns = new[]
            { "#", "name", "role", "address", "tel", "email", "maintainerYN", "technicalLevel" };
        var rows = new List<string[]>()
        {
            new[]{"1", "test", "cto", "test", "000-111-2222", "test@test.com", "Y", "S"},
        };

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .Start(ctx =>
            {
                columns.xForEach(m =>
                {
                    table.AddColumn(m);
                    ctx.Refresh();
                    //Thread.Sleep(1000);
                });
                
                rows.xForEach(m =>
                {
                    table.AddRow(m);
                    ctx.Refresh();
                });
            });

        await AnsiConsole.Live(table)
            .AutoClear(true)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .StartAsync(async ctx =>
            {
                // await columns.xForEachAsync(async m =>
                // {
                //     table.AddColumn(m);
                //     ctx.Refresh();
                //     await Task.Delay(100);
                // });
                //
                await rows.xForEachAsync(async m =>
                {
                    table.AddRow(m);
                    ctx.Refresh();
                    await Task.Delay(100);
                });
            });


        #endregion
    }
}