using System.Collections.Generic;

namespace JUIControls;

/// <summary>
/// sale select box 만들기
/// </summary>
public class SaleSelectBoxWidget : Widget
{
    private readonly IWidgetBinder _widgetBinder;
    private readonly Dictionary<string, string> _options;
    public SaleSelectBoxWidget(IWidgetBinder widgetBinder, 
        Dictionary<string, string> options)
    {
        _widgetBinder = widgetBinder;
        _options = options;
        
        this.Bind();
    }

    protected override void Bind()
    {
        var valueAndOptions = _widgetBinder.Bind(_options);
        this.Value = valueAndOptions.value;
        this.ValueOptions = valueAndOptions.valueOptions;
    }
}