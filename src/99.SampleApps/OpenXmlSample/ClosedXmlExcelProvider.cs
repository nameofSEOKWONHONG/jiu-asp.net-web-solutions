using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public sealed class ClosedXmlExcelProvider : IExcelProvider
{
    private readonly SpreadsheetDataHandler _handler;
    public ClosedXmlExcelProvider()
    {
        _handler = new SpreadsheetDataHandler();
    }

    public void CreateExcel(string filePath, SpreadsheetData data)
    {
        var dataFormat = data;
        
        using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
        {
            var worksheet = workbook.Worksheets.Add(dataFormat.SheetTitle);
            
            #region [set value header and contents]
            
            dataFormat.Header.Columns.xForEach((col, i) =>
            {
                var cell = worksheet.Row(1).Cell(i+1);
                cell.Value = col.Cell.Value; 
            });
            
            dataFormat.Rows.xForEach((dataRow, i) =>
            {
                //worksheet.RowHeight = row.Height;
                var row = worksheet.Row(i + 2);
                dataRow.Columns.xForEach((dataCol, j) =>
                {
                    var cell = worksheet.Cell(i+2, j+1);
                    if(dataCol.Cell.CellType == CellType.Text) cell.Value = dataCol.Cell.Value;
                    else if (dataCol.Cell.CellType == CellType.Formula)
                        cell.FormulaA1 = dataCol.Cell.Value;
                    else throw new NotImplementedException();
                });
            });
            
            #endregion

            #region [set style header and contents]

            SetHeaderStyle(ref worksheet, dataFormat);
            SetContentsStyle(ref worksheet, dataFormat);            

            #endregion
            
            workbook.SaveAs(filePath);
        }
    }

    /// <summary>
    /// header 영역에 대한 스타일링 수행
    /// </summary>
    /// <param name="worksheet"></param>
    private void SetHeaderStyle(ref IXLWorksheet worksheet, SpreadsheetData data)
    {
        var ws = worksheet;
        data.Header.Columns.xForEach((col, i) =>
        {
            var cell = ws.Cell(1, i + 1);
            //cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(cellItem.CellRgba));
            cell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            cell.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        });
    }

    /// <summary>
    /// contents 영역에 대한 스타일링 수행
    /// </summary>
    /// <param name="worksheet"></param>
    /// <param name="data"></param>
    private void SetContentsStyle(ref IXLWorksheet worksheet, SpreadsheetData data)
    {
        var ws = worksheet;
        var row = data.Rows.xFirst();
        row.Columns.xForEach((col, i) =>
        {
            ws.Column(i+1).Width = col.Width;
        });
    }
}