// using System.Security.AccessControl;
// using ClosedXML.Excel;
// using eXtensionSharp;
// using FastExcel;
//
// namespace OpenXmlSample;
//
// public class FastExcelProvider : IExcelProvider
// {
//     public void CreateExcel(string filePath, SpreadsheetDataFormat dataFormat)
//     {
//         FileInfo outputFile = new FileInfo(filePath);
//         FileInfo templateFile = new FileInfo("D://Template.xlsx");
//
//         var startAlpha = 65;
//         var rowIndex = 1;
//         var colIndex = 1;
//         
//         var worksheet = new Worksheet();
//         var rows = new List<FastExcel.Row>();
//         var cells = new List<FastExcel.Cell>();
//         dataFormat.Headers.xForEach(header =>
//         {
//             cells.Add(new FastExcel.Cell(colIndex, header));
//             colIndex += 1;
//         });
//         rows.Add(new FastExcel.Row(rowIndex, cells));
//
//         dataFormat.Rows.xForEach(row =>
//         {
//             rowIndex += 1;
//             colIndex = 1;
//             var newCells = new List<FastExcel.Cell>();
//             row.Cells.xForEach(cell =>
//             {
//                 var fastExcelCell = new FastExcel.Cell(colIndex, cell.Value);
//                 newCells.Add(fastExcelCell);
//                 colIndex += 1;
//             });
//             var fastExcelRow = new FastExcel.Row(rowIndex, newCells);
//             rows.Add(fastExcelRow);
//         });
//         worksheet.Rows = rows;
//         using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(templateFile, outputFile))
//         {
//             fastExcel.Write(worksheet, "sheet1");
//         }
//     }
// }