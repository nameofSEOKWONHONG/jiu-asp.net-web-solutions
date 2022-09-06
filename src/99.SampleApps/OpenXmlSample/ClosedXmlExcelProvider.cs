using ClosedXML.Excel;
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

    #region [data handler]

    public void SetSheetTitle(string title) => this._handler.SetSheetTitle(title);
    public void SetHeader(IEnumerable<string> headers) => this._handler.SetHeader(headers.ToArray());

    public void SetContents(List<String[]> values, List<CellType> cellTypes = null,
        List<AlignmentType> alignmentTypes = null)
        => this._handler.SetContents(values, cellTypes, alignmentTypes);    

    #endregion

    public void CreateExcel(string filePath)
    {
        var dataFormat = _handler.Data;
        
        using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
        {
            var worksheet = workbook.Worksheets.Add("test sheet");
            
            #region [set value header and contents]
            
            dataFormat.Header.Columns.xForEach((col, i) =>
            {
                worksheet.Cell($"{col.ColName}1").Value = col.Cell.Value;
            });
            
            dataFormat.Rows.xForEach((row, i) =>
            {
                //worksheet.RowHeight = row.Height;
                row.Columns.xForEach((col, j) =>
                {
                    var rowCell = worksheet.Cell(i+2, j+1);
                    if(col.Cell.CellType == CellType.Text) rowCell.Value = col.Cell.Value;
                    else if (col.Cell.CellType == CellType.Formula)
                        rowCell.FormulaA1 = col.Cell.Value;
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
        row.Columns.xForEach(col =>
        {
            ws.Column(col.ColName).Width = col.Width;
        });
    }
}