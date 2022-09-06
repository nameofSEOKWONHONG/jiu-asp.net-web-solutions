using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public class SpreadsheetDataHandler
{
    private readonly SpreadsheetData _spreadsheetData;
    public SpreadsheetData Data => _spreadsheetData;
    public SpreadsheetDataHandler()
    {
        if (_spreadsheetData.xIsEmpty()) _spreadsheetData = new SpreadsheetData();
    }

    public void SetSheetTitle(string title) => _spreadsheetData.SheetTitle = title;

    public void SetHeader(string[] headers)
    {
        var row = new Row();
        row.Columns = new List<Col>();
        var alphabet = 65;
        headers.xForEach(header =>
        {
            row.Columns.Add(new Col()
            {
                ColName = GetColName(ref alphabet),
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

    public void SetContents(List<String[]> values, List<CellType> cellTypes = null, List<AlignmentType> alignmentTypes = null)
    {
        CellType defaultCellType = CellType.Text;
        AlignmentType defaultAlignmentType = AlignmentType.Left;

        _spreadsheetData.Rows = new List<Row>();
        values.xForEach((value, i) =>
        {
            var row = new Row();
            row.Columns = new List<Col>();
            value.xForEach((v, j) =>
            {
                row.Columns.Add(new Col()
                {
                    ColName = _spreadsheetData.Header.Columns[j].ColName,
                    Width = 100,
                    Cell = new Cell()
                    {
                        CellType = cellTypes.xIsEmpty() ? defaultCellType : cellTypes[i],
                        Value = v,
                        AlignmentType = alignmentTypes.xIsEmpty() ? defaultAlignmentType : alignmentTypes[i],                        
                    }
                });
            });
            _spreadsheetData.Rows.Add(row);
        });
    }
}











