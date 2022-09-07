using OpenXmlSample.Data;

namespace OpenXmlSample;

public sealed class SpreadsheetData
{
    public string SheetTitle { get; set; } = "sheet1";
    public Row Header { get; set; }
    public List<Row> Rows { get; set; }
}

