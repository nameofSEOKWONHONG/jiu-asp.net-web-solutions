// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BenchmarkDotNet.Running;
using eXtensionSharp;
using OpenXmlSample;
using OpenXmlSample.Data;

Console.WriteLine("Excel Export Test");
// List<IExcelProvider> excelProviders = new List<IExcelProvider>()
// {
//     new ClosedXmlExcelProvider(),
//     //new FastExcelProvider()
// };

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

BenchmarkRunner.Run<ClosedXmlExcelProviderBenchmark>();

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

// Console.WriteLine("=======================================END=======================================");
// Console.ReadLine();