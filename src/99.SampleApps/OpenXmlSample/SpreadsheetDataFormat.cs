using eXtensionSharp;

namespace OpenXmlSample;

public class SpreadsheetDataFormatHandler
{
    private readonly SpreadsheetDataFormat _spreadsheetDataFormat;
    public SpreadsheetDataFormatHandler(SpreadsheetDataFormat spreadsheetDataFormat)
    {
        _spreadsheetDataFormat = spreadsheetDataFormat;
    }

    public IEnumerable<String> GetColumnNames()
    {
        var startAlpha = 65;
        var colNames = new List<String>();
        _spreadsheetDataFormat.Headers.xForEach(header =>
        {
            colNames.Add(((char)startAlpha).ToString());
            startAlpha += 1;
            if (startAlpha >= 91)
            {
                throw new Exception("not support range.");
            }
        });
        return colNames;
    }
}

public class SpreadsheetDataFormat
{
    public List<String> Headers { get; set; }
    public List<Row> Rows { get; set; }
}

public class Row
{
    public List<Cell> Cells { get; set; }
    public int Height { get; set; }
}

public class Cell
{
    public string Value { get; set; }
    public CellType CellType { get; set; }
    public int Width { get; set; }
}

public enum CellType
{
    Text,
    Formula,
}