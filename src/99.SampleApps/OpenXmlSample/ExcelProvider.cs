using System.Data;
using ClosedXML.Excel;
using eXtensionSharp;

namespace OpenXmlSample;

public class ExcelProvider
{
    public ExcelProvider()
    {

    }

    public void CreateExcel(string filePath, SpreadsheetDataFormat dataFormat)
    {
        using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
        {
            var workseet = workbook.Worksheets.Add("test sheet");
            var startAlpha = 65;
            var rowCnt = 1;
            dataFormat.Headers.xForEach((header, i) =>
            {
                workseet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").Value = header;
                startAlpha += 1;
            });
            dataFormat.Rows.xForEach((row, i) =>
            {
                startAlpha = 65;
                rowCnt += 1;
                row.Cells.xForEach((cell, j) =>
                {
                    if(cell.CellType == CellType.Text) workseet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").Value = cell.Value;
                    else if (cell.CellType == CellType.Formula)
                        workseet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").FormulaA1 = cell.Value;
                    startAlpha += 1;
                });
            });
            // workseet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";
            workbook.SaveAs(filePath);
        }
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
}

public class Cell
{
    public string Value { get; set; }
    public CellType CellType { get; set; }
}

public enum CellType
{
    Text,
    Formula,
}