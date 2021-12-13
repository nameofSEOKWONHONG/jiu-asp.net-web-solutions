using System.Collections.Generic;

namespace JUIControls.Services;

/// <summary>
/// dummy interface
/// </summary>
public interface ISaleService : IWidgetBinder
{
    string GetSale(int id);
    Dictionary<string, string> GetSaleValueOptions(string comCode);
}