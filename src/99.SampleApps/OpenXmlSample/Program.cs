// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using eXtensionSharp;
using OpenXmlSample;
using OpenXmlSample.Data;

Console.WriteLine("Excel Export Test");
List<IExcelProvider> excelProviders = new List<IExcelProvider>()
{
    new ClosedXmlExcelProvider(),
    //new FastExcelProvider()
};

// var headers = new List<Row>();
// Enumerable.Range(1, 30).ToList().ForEach(i =>
// {
//     var row = new Row()
//     {
//         Cells = new()
//         {
//             new Cell() { Value = i.ToString(), Width = 100, CellType = CellType.Text }
//         }
//     };
//     headers.Add(row);
// });
// var rows = new List<OpenXmlSample.Row>();
// Enumerable.Range(1, 2500).ToList().ForEach(i =>
// {
//     var cells = new List<Cell>();
//     Enumerable.Range(1, 30).ToList().ForEach(j =>
//     {
//         cells.Add(new Cell() {CellType = CellType.Text, Value = $"hello world{j}"});
//     });
//     rows.Add(new Row() {Cells = cells});
// });

Enumerable.Range(1, 10).ToList().ForEach(i =>
{
    var datum = new List<String[]>();
    Enumerable.Range(1, 300).ToList().ForEach(i =>
    {
        var data = new List<String>();
        Enumerable.Range(1, 25).ToList().ForEach(j =>
        {
            data.Add($"test{j}");
        });
        datum.Add(data.ToArray());
    });
    var idata = new List<string>();
    datum.Add(Enumerable.Range(1, 25).Select(i => $"0.{i}").ToArray());
    datum.Add(Enumerable.Range(1, 25).Select(i => $"https://naver.com").ToArray());
    
    var sw = Stopwatch.StartNew();
    sw.Start();
    var handler = new SpreadsheetDataHandler();
    handler.SetSheetTitle(() => "sheet");
    handler.SetHeader(() => Enumerable.Range(1, 25).Select(m => m.ToString()).ToArray());
    handler.SetContents(() => new Contents(){ Values = datum, AlignmentTypes = null, CellTypes = null});
    var data = handler.Execute();
    excelProviders[0].CreateExcel($"d://{Guid.NewGuid().ToString("N")}.xlsx", data);
    sw.Stop();
    Console.WriteLine($"============================result({i})============================");
    Console.WriteLine(sw.Elapsed.TotalSeconds);
});

#region [compare code]

// Console.WriteLine("===============================CloseXml Excel Export===============================");
// Enumerable.Range(1, 10).ToList().ForEach(i =>
// {
//     var sw = Stopwatch.StartNew();
//     sw.Start();
//     excelProviders[0].CreateExcel($"d://{i}.xlsx", spreedSheetDataFormat);
//     sw.Stop();
//     Console.WriteLine($"============================result({i})============================");
//     Console.WriteLine(sw.Elapsed.TotalSeconds);
// });
//
// Console.WriteLine("===============================FastExcel Export===============================");
// Enumerable.Range(1, 10).ToList().ForEach(i =>
// {
//     var sw = Stopwatch.StartNew();
//     sw.Start();
//     excelProviders[1].CreateExcel($"d://{i}{i}.xlsx", spreedSheetDataFormat);
//     sw.Stop();
//     Console.WriteLine($"============================result({i}{i})============================");
//     Console.WriteLine(sw.Elapsed.TotalSeconds);
// });
//
// Console.WriteLine("===============================Parallel Export===============================");
// var sw = Stopwatch.StartNew();
// sw.Start();
// Parallel.ForEach(Enumerable.Range(1, 10).ToList(), i =>
// {
//     excelProviders[0].CreateExcel($"d://x{i}.xlsx", spreedSheetDataFormat);
// });
// sw.Stop();
// Console.WriteLine($"CloseXml : {sw.Elapsed.TotalSeconds}");
//
// sw.Reset();
// sw.Start();
// Parallel.ForEach(Enumerable.Range(1, 10).ToList(), i =>
// {
//     excelProviders[1].CreateExcel($"d://x{i}{i}.xlsx", spreedSheetDataFormat);
// });
// sw.Stop();
// Console.WriteLine($"FastExcel : {sw.Elapsed.TotalSeconds}");

#endregion

Console.WriteLine("=======================================END=======================================");
Console.ReadLine();