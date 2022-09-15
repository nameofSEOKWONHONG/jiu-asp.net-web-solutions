using ClosedXML.Excel;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public sealed class ClosedXmlExcelProvider : IExcelProvider, IDisposable
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

    private const string TEMPLATE_FILE_PATH = "d://template.xlsx";
    private bool _disposed = false;
    private readonly XLWorkbook _workbook;
    
    public ClosedXmlExcelProvider(string filePath)
    {
        filePath.xIfEmpty(() => throw new Exception("file path is empty."));
        //copy src styling excel file
        File.Copy(TEMPLATE_FILE_PATH, filePath);
        _workbook = new XLWorkbook(filePath, XLEventTracking.Disabled);
    }

    #region [public]
    public void CreateExcel(SpreadsheetData data)
    {
        var dataFormat = data;
        
        //overwrite
        //using ()
        //{
            var worksheet = _workbook.Worksheets.First();
            
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

            //스타일을 지정할 경우 성능이 매우 떨어지므로 미리 스타일링 되어 있는 파일을 사용한다.
            //width 범위를 넘어서는 text width는 width 설정을 지원하지 않는다.
            //스타일 지정을 별도로 사용하지 않는다.
            //SetHeaderStyle(ref worksheet, dataFormat);
            //SetContentsStyle(ref worksheet, dataFormat);

            #endregion
            
            // 메모리 스트림으로 성능향상은 미미함.
            // 대량 반복 수행시에 문제될 경우 사용해보자.
            // 단순 export라면 fastxml을 사용.
            // using (MemoryStream memoryStream = SaveWorkbookToMemoryStream(workbook))
            // {
            //     File.WriteAllBytes(_filePath, memoryStream.ToArray());
            // }
            _workbook.Save();
        //}
    }

    public void CreateExcel(SpreadsheetDatum datum)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region [private]
    
    private MemoryStream SaveWorkbookToMemoryStream(XLWorkbook workbook)
    {
        using MemoryStream stream = new MemoryStream();
        workbook.SaveAs(stream, new SaveOptions { EvaluateFormulasBeforeSaving = false, GenerateCalculationChain = false, ValidatePackage = false });
        return stream;
    }
    
    /// <summary>
    /// header 영역에 대한 스타일링 수행
    /// </summary>
    /// <param name="worksheet"></param>
    [Obsolete("don't use", true)]
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
    [Obsolete("don't use", true)]
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

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // OK to use any private object references
                _workbook.Dispose();
            }
            //release unmanaged resources.
            //set large fields to null.
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #region [deconstructor]

    ~ClosedXmlExcelProvider()
    {
        Dispose(false);
    }

    #endregion
    
}