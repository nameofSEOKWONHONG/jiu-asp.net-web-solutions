using System.Data;
using ClosedXML.Excel;
using eXtensionSharp;

namespace OpenXmlSample;

public interface IExcelProvider
{
    void CreateExcel(string filePath, SpreadsheetDataFormat dataFormat);
}

