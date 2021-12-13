using System.Collections.Generic;
using System.Globalization;
using JUIControls.Services;

namespace JUIControls;

/// <summary>
/// 판매 select box 구현
/// </summary>
public class SaleSelectBoxWidgetBinder : IWidgetBinder
{
    /// <summary>
    /// 서비스 또는 Repository
    /// </summary>
    private readonly ISaleService _saleService;
    public SaleSelectBoxWidgetBinder(ISaleService saleService)
    {
        _saleService = saleService;
    }

    public (string value, Dictionary<string, string> valueOptions) Bind(Dictionary<string, string> options)
    {
        return _saleService.Bind(options);
    }
}