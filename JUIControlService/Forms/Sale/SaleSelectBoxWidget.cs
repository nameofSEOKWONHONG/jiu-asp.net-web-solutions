using System;
using System.Collections.Generic;
using eXtensionSharp;
using JUIControlService.Widget;

namespace JUIControlService.Forms.Sale;

/// <summary>
/// sale select box 만들기
/// </summary>
public class SaleSelectBoxWidget : WidgetBase
{
    public SaleSelectBoxWidget(IWidgetBinder widgetBinder, Dictionary<string, string> options)
    {
        widgetBinder.xIfEmptyThrow(() => "widgetbinder is null");
        options.xIfEmptyThorw(() => new NullReferenceException("options is null"));
        
        var valueAndOptions = widgetBinder.Bind(options);
        this.Value = valueAndOptions.value;
        this.ValueOptions = valueAndOptions.valueOptions;
    }
}