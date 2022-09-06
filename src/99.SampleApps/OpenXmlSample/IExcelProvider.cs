using System.Data;
using ClosedXML.Excel;
using eXtensionSharp;
using OpenXmlSample.Data;

namespace OpenXmlSample;

public interface IExcelProvider
{
    void SetSheetTitle(string title);
    void SetHeader(IEnumerable<string> headers);
    void SetContents(List<String[]> values, List<CellType> cellTypes = null,
        List<AlignmentType> alignmentTypes = null);
    /// <summary>
    /// 엑셀파일 생성
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataFormat"></param>
    void CreateExcel(string filePath);
}

