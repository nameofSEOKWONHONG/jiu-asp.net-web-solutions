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

/**
 *  result : c#(closedxml)
 *  Excel Export Test
    ============================result(1)============================
    0.6610683
    ============================result(2)============================
    0.0929961
    ============================result(3)============================
    0.0851467
    ============================result(4)============================
    0.0854137
    ============================result(5)============================
    0.0690752
    ============================result(6)============================
    0.0665898
    ============================result(7)============================
    0.0642861
    ============================result(8)============================
    0.0670824
    ============================result(9)============================
    0.0615653
    ============================result(10)============================
    0.0668502
    
    result : java(poi)
    0.490395
    0.1592382
    0.1348167
    0.1214288
    0.1239471
    0.122226
    0.1189653
    0.1202132
    0.1167813
    0.1192532    
 */

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

//Console.ReadLine();

/*
1. Database에서 생성할 엑셀파일 내역을 추출하여 서버 스토리지에 파일 작성(예:txt, csv)
2. 스케줄러가 파일을 읽어 데이터 생성 및 엑셀 출력.
3. 출력된 엑셀 파일 및 첨부파일을 압축하여 스토리지에 업로드
4. 압축된 첨부파일을 메일 발송.
5. 압축파일 및 생성 파일들 삭제.
** 각 파일 생성은 독자 디렉토리 생성하여 진행 **
** 작업 완료된 경우 일괄 삭제 **
** hangfire나 qutarz 사용하여 스케줄러로 1차 구현. (서버측에서) **
** 이후 worker 서버 별도 구성할 수 있을 경우 kafka등을 이용한다. **
** 마감 이후에 재 정산 또는 재 다운로드 할 경우 유료 서비스로 제공할지...(횟수는 10회 까지만)      **
** 보관 기간은 5년      **
 *
 * 구현해본바, CloseXml과 FastExcel은 약 9~10배 성능차이.
 * 파일을 직접 생성하는 것과 읽어들인 Template파일에서 생성하는 것과 차이가 있는 것으로 생각됨.
 * DataTable, DataSet을 사용하는 것보다 직접 맵핑하는게 더 빠르다.
 * FastExcel의 치명적 문제는 서식 적용이 안됨. 원본에 서식이 적용되더라도 최종결과물에서 서식 적용이 안됨.
 * 빠른 결과 만을 원한다면 FastExcel, 각종 서식과 Excel 지원을 필요로 한다면 CloseXml, Native에 가깝게 개발하고 싶다면 OpenXmlSdk를 사용하자.
 * CloseXml은 OpenXmlSdk Wrapper이다.
*/