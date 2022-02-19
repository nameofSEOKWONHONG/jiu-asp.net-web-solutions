using Spectre.Console;

namespace SpectreConsoleSample;

public class WidgetSample : ISample
{
    public WidgetSample()
    {
        
    }

    public void Run()
    {
        var table = new Table();
        table.AddColumn("Foo");
        table.AddColumn(new TableColumn("Bar").Centered());
        table.AddColumn("state");

        table.AddRow("Baz", "[green]Qux[/]");
        table.AddRow(new Markup("[blue]Cogi[/]"), new Panel("Waldo"));
        table.AddRow("", "", "");
        
        table.Border(TableBorder.None);
        table.UpdateCell(0, 2, "TableBorder None");
        AnsiConsole.Write(table);

        table.Border(TableBorder.Ascii);
        table.UpdateCell(0, 2, "TableBorder Ascii");
        AnsiConsole.Write(table);

        table.Border(TableBorder.Square);
        table.UpdateCell(0, 2, "TableBorder Square");
        AnsiConsole.Write(table);

        table.Border(TableBorder.Rounded);
        table.UpdateCell(0, 2, "TableBorder Rounded");
        AnsiConsole.Write(table);
        
        table.Expand();
        table.UpdateCell(0, 2, "Table Expand");
        AnsiConsole.Write(table);

        table.Collapse();
        table.UpdateCell(0, 2, "Table Collapse");
        AnsiConsole.Write(table);
    }
}