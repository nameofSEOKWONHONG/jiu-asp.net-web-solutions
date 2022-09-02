using ClosedXML.Excel;
using eXtensionSharp;

namespace OpenXmlSample;

public class ClosedXmlExcelProvider : IExcelProvider
{
    public ClosedXmlExcelProvider()
    {

    }

    public void CreateExcel(string filePath, SpreadsheetDataFormat dataFormat)
    {
        using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
        {
            var worksheet = workbook.Worksheets.Add("test sheet");
            var startAlpha = 65;
            var rowCnt = 1;
            dataFormat.Headers.xForEach((header, i) =>
            {
                worksheet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").Value = header;
                startAlpha += 1;
            });
            dataFormat.Rows.xForEach((row, i) =>
            {
                startAlpha = 65;
                rowCnt += 1;
                row.Cells.xForEach((cell, j) =>
                {
                    if(cell.CellType == CellType.Text) worksheet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").Value = cell.Value;
                    else if (cell.CellType == CellType.Formula)
                        worksheet.Cell($"{((char)startAlpha).ToString()}{rowCnt}").FormulaA1 = cell.Value;
                    startAlpha += 1;
                });
            });
            // workseet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";

            SetWorksheetStyle(ref worksheet, ref dataFormat);
            
            workbook.SaveAs(filePath);
        }
    }

    /// <summary>
    /// 완성된 Worksheet를 바탕으로 스타일을 적용한다.
    /// </summary>
    /// <param name="worksheet"></param>
    private void SetWorksheetStyle(ref IXLWorksheet worksheet, ref SpreadsheetDataFormat dataFormat)
    {
        var ws = worksheet;
        var handler = new SpreadsheetDataFormatHandler(dataFormat);
        handler.GetColumnNames().xForEach((colName, i) =>
        {
            var cell = ws.Cell(1, i + 1);
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder =XLBorderStyleValues.Thin; 
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            //ws.Column(colName).Width = 200;
            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        });
    }
}