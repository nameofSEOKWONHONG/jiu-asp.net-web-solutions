using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public class SpreadsheetDataHandler
{
    private readonly SpreadsheetData _spreadsheetData;
    public SpreadsheetDataHandler()
    {
        if (_spreadsheetData.xIsEmpty()) _spreadsheetData = new SpreadsheetData();
    }

    private Func<String> _sheetTitleFunc = null;
    private Func<IEnumerable<String>> _headerFunc = null;
    private Func<Contents> _contentsFunc = null;
    
    public SpreadsheetData Execute()
    {
        if (_sheetTitleFunc.xIsNotEmpty())
        {
            var sheetTitle = _sheetTitleFunc();
            this.SetSheetTitle(sheetTitle);
        }
        if (_headerFunc.xIsNotEmpty())
        {
            var headers = _headerFunc();
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
        _headerFunc = func;
        return this;
    }
    public SpreadsheetDataHandler SetContents(Func<Contents> func)
    {
        _contentsFunc = func;
        return this;
    }

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
                        CellType = arrCellTypes.xIsEmpty() ? CellType.Text : arrCellTypes[j],
                        Value = v,
                        AlignmentType = v.xIsNumber() ? AlignmentType.Right : AlignmentType.Left //arrAlignmentTypes.xIsEmpty() ? AlignmentType.Right : arrAlignmentTypes[j]                        
                    }
                });
            });
            _spreadsheetData.Rows.Add(row);
        });
    }
}











