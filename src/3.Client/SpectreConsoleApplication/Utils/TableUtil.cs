using eXtensionSharp;
using Spectre.Console;

namespace SpectreConsoleApplication.Utils;

public class TableUtil
{
    public static void TableDraw<T>(IEnumerable<T> list, Action<Table, T> rowHandler = null) where T : class
    {
        var table = new Table();
        AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .Start(ctx =>
            {
                var columns = new List<string>();
                typeof(T).GetProperties().xForEach(prop =>
                {
                    columns.Add(prop.Name);
                });
                table.AddColumns(columns.ToArray());
                ctx.Refresh();
                
                list.xForEach((item, i) =>
                {
                    if (rowHandler.xIsNotEmpty())
                    {
                        rowHandler(table, item);
                    }
                    else
                    {
                        var map = item.xToDictionary();
                        var row = new List<string>();
                        columns.xForEach(column =>
                        {
                            row.Add(map[column].xValue<string>());
                        });
                        table.AddRow(row.ToArray());                        
                    }
                    ctx.Refresh();
                });
            });
    }
}