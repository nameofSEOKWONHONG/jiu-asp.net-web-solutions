using System.Diagnostics;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public class SpreadsheetDataHandler
{
    private Func<String> _sheetTitleFunc = null;
    private Func<IEnumerable<String>> _valueHeaderFunc = null;
    private Func<IEnumerable<Row>> _rowHeaderFunc = null;
    private Func<Contents> _contentsFunc = null;
    private readonly SpreadsheetData _spreadsheetData;
    
    public SpreadsheetDataHandler()
    {
        if (_spreadsheetData.xIsEmpty()) _spreadsheetData = new SpreadsheetData();
    }

    #region [public]

    public SpreadsheetData Execute()
    {
        if (_sheetTitleFunc.xIsNotEmpty())
        {
            var sheetTitle = _sheetTitleFunc();
            this.SetSheetTitle(sheetTitle);
        }
        if (_valueHeaderFunc.xIsNotEmpty())
        {
            var headers = _valueHeaderFunc();
            this.SetHeader(headers);
        }

        if (_rowHeaderFunc.xIsNotEmpty())
        {
            var headers = _rowHeaderFunc();
            this.SetHeader(headers);
        }

        if (_contentsFunc.xIsNotEmpty())
        {
            var contents = _contentsFunc();
            this.SetContents(contents.Values, contents.CellTypes, contents.AlignmentTypes);
        }

        return _spreadsheetData;
    }
    
    public SpreadsheetDataHandler SetSheetTitle(Func<String> func)
    {
        _sheetTitleFunc = func;
        return this;
    }
    
    public SpreadsheetDataHandler SetHeader(Func<IEnumerable<String>> func)
    {
        _valueHeaderFunc = func;
        return this;
    }

    public SpreadsheetDataHandler SetHeader(Func<IEnumerable<Row>> func)
    {
        _rowHeaderFunc = func;
        return this;
    }
    
    public SpreadsheetDataHandler SetContents(Func<Contents> func)
    {
        _contentsFunc = func;
        return this;
    }

    #endregion

    #region [private]

    private void SetSheetTitle(string title) => _spreadsheetData.SheetTitle = title;
    
    private void SetHeader(IEnumerable<string> headers)
    {
        var row = new Row();
        row.Columns = new List<Col>();
        headers.xForEach(header =>
        {
            row.Columns.Add(new Col()
            {                
                Width = 100,
                Cell = new Cell()
                {
                    Value = header,
                    CellType = CellType.Text,
                    AlignmentType = AlignmentType.Center,
                } 
            });
        });
        _spreadsheetData.Header = row;
    }

    private void SetHeader(IEnumerable<Row> headers)
    {
        _spreadsheetData.MultiHeader = headers;
    }
    
    /// <summary>
    /// return A, B, C...
    /// </summary>
    /// <param name="alphabetNum"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("don't use", true)]
    private string GetColName(ref int alphabetNum)
    {
        var format = ((char)alphabetNum).ToString();
        if (alphabetNum >= 65 && alphabetNum <= 90)
        {
            alphabetNum += 1;    
        }
        else
        {
            throw new NotSupportedException();
        }
        // else if (alphabetNum >= 91)
        // {
        //     alphabetNum -= 26;
        //     return $"{((char)65).ToString()}{((char)alphabetNum).ToString()}";
        // }

        return format;
    }    
    
    private void SetContents(IEnumerable<IEnumerable<String>> values, IEnumerable<CellType> cellTypes = null, IEnumerable<AlignmentType> alignmentTypes = null)
    {
        _spreadsheetData.Rows = new List<Row>();
        var arrCellTypes = cellTypes?.ToArray();
        var arrAlignmentTypes = alignmentTypes?.ToArray();
        values.xForEach((value, i) =>
        {
            var row = new Row();
            row.Columns = new List<Col>();
            value.xForEach((v, j) =>
            {
                row.Columns.Add(new Col()
                {
                    Width = 100,
                    Cell = new Cell()
                    {
                        CellType = GetCellType(v),
                        Value = v,
                        AlignmentType = GetAlignmentType(v)                        
                    }
                });
            });
            _spreadsheetData.Rows.Add(row);
        });
    }

    private CellType GetCellType(string v)
    {
        if (v.xGetFirst(4).xIsEquals("http")) return CellType.HyperLink;
        else if (v.xFirst().xIsEquals("=")) return CellType.Formula;
        return CellType.Text;
    }

    private AlignmentType GetAlignmentType(string v)
    {
        if (v.xIsNumber()) return AlignmentType.Right;
        return AlignmentType.Left;
    }

    #endregion
    
}











