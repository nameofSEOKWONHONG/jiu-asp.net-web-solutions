using System.Diagnostics;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public sealed class ClosedXmlExcelProvider : IExcelProvider
{
    private readonly Dictionary<CellType, Action<IXLCell, Col>> _spreadsheetCellTypeSettingStates =
        new()
        {
            {
                CellType.Text, (cell, col) =>
                {
                    cell.Value = col.Cell.Value;
                }
            },
            {
                CellType.Formula, (cell, col) =>
                {
                    cell.FormulaA1 = col.Cell.Value;
                }
            },
            {
                CellType.HyperLink, (cell, col) =>
                {
                    cell.Value = col.Cell.Value;
                    cell.SetHyperlink(new XLHyperlink(col.Cell.Value));
                }
            }
        };
    private readonly string TemplatePath = "d://template.xlsx";
    
    public ClosedXmlExcelProvider()
    {
    }

    #region [public]

    public void CreateExcel(string filePath, SpreadsheetData data)
    {
        //1. 템플릿파일 복사 및 읽기 (구현결과에 따라 달라질 수 있음.)
        //2. worksheet 작성 (템플릿에 스타일 지정이 모두 되어 있어야 함)
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
                    var state = _spreadsheetCellTypeSettingStates[dataCol.Cell.CellType];
                    state(cell, dataCol);
                });
            });
            
            #endregion

            #region [set style header and contents]

            //스타일 지정을 별도로 사용하지 않는다.
            //SetHeaderStyle(ref worksheet, dataFormat);
            //SetContentsStyle(ref worksheet, dataFormat);            

            #endregion
            
            workbook.SaveAs(filePath);
        }
    }

    public void CreateExcel(string filePath, SpreadsheetDatum datum)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region [private]
    
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
    
    #endregion

    /// <summary>
    /// template 기반 excel create...
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="action"></param>
    private IXLWorksheet ReadExcel(string filePath, Action<IXLWorksheet> action)
    {
        IXLWorksheet worksheet = null;
        using (var workbook = new XLWorkbook(filePath, XLEventTracking.Disabled))
        {
            worksheet = workbook.Worksheet(1);
        }

        return worksheet;
    }
}