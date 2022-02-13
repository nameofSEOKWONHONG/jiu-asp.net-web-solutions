using System.Collections.Generic;
using System.Globalization;
using JUIControlService.Binders;
using JUIControlService.Widget;

namespace JUIControlService.Forms.Sale;

/// <summary>
/// 판매 select box 구현
/// </summary>
public class SaleSelectBoxWidgetBinder : IWidgetBinder
{
    /// <summary>
    /// 서비스 또는 Repository
    /// </summary>
    private readonly ISaleWidgetBinder _saleWidgetBinder;
    public SaleSelectBoxWidgetBinder(ISaleWidgetBinder saleWidgetBinder)
    {
        _saleWidgetBinder = saleWidgetBinder;
    }

    public (string value, Dictionary<string, object> valueOptions) Bind(Dictionary<string, string> options)
    {
        return _saleWidgetBinder.Bind(options);
    }
}