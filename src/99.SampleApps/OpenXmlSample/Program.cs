// See https://aka.ms/new-console-template for more information

using OpenXmlSample;

Console.WriteLine("Hello, World!");

var excelProvider =  new ExcelProvider();
var spreedSheetDataFormat = new SpreadsheetDataFormat()
{
    Headers = new List<String>()
    {
        "T1", "T2", "T3"
    },
    Rows = new List<Row>()
    {
        new Row()
        {
            Cells = new List<Cell>()
            {
                new Cell() {CellType = CellType.Text, Value = "Hello World!"},
                new Cell() {CellType = CellType.Text, Value = "test2"},
                new Cell() {CellType = CellType.Formula, Value = "=MID(A2, 7, 5)"}
            }
        }
    }
};

excelProvider.CreateExcel("d://test.xlsx", spreedSheetDataFormat);