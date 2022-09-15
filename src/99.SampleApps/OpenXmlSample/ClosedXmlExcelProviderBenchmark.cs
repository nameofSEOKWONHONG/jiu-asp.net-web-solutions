using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using OpenXmlSample.Data;

namespace OpenXmlSample;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ClosedXmlExcelProviderBenchmark
{
    [Benchmark]
    public void Runner()
    {
        var datum = new List<String[]>();
        Enumerable.Range(1, 500).ToList().ForEach(i =>
        {
            var data = new List<String>();
            Enumerable.Range(1, 25).ToList().ForEach(j =>
            {
                data.Add($"test{j + "000909234"}");
            });
            datum.Add(data.ToArray());
        });
        datum.Add(Enumerable.Range(1, 25).Select(i => $"0.{i + "00909213"}").ToArray());
        datum.Add(Enumerable.Range(1, 25).Select(i => $"https://naver.com").ToArray());
        
        var handler = new SpreadsheetDataHandler();
        handler.SetSheetTitle(() => "sheet");
        handler.SetHeader(() => Enumerable.Range(1, 25).Select(m => "909829838787"+m.ToString()).ToArray());
        handler.SetContents(() => new Contents(){ Values = datum, AlignmentTypes = null, CellTypes = null});
        var data = handler.Execute();
        using (var excelProvider = new ClosedXmlExcelProvider($"d://{Guid.NewGuid():N}.xlsx"))
        {
            excelProvider.CreateExcel(data);    
        }
    }
}