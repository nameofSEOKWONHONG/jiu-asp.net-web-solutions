using System.Data;
using ClosedXML.Excel;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public interface IExcelProvider
{
    /// <summary>
    /// 엑셀파일 생성
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataFormat"></param>
    void CreateExcel(SpreadsheetData data);

    void CreateExcel(SpreadsheetDatum datum);
}

