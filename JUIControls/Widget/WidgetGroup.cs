using System.Collections.Generic;
using System.Data;
using System.Linq;
using eXtensionSharp;

namespace JUIControls;

/// <summary>
/// 컨트롤 그룹
/// </summary>
public class WidgetGroup
{
    /// <summary>
    /// 그룹 Name (빈값일 경우 외곽선 및 명칭 표기가 되지 않는다.)
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// 마스터 유뮤, 마스터는 무조건 왼쪽에 배치한다.
    /// </summary>
    public bool IsMaster { get; set; } = false;

    /// <summary>
    /// 다음 컨트롤 신규 라인 여부
    /// </summary>
    public bool NewLine { get; set; } = false;

    public List<WidgetBase> Widgets { get; private set; } = new List<WidgetBase>();

    public WidgetGroup(List<WidgetBase> widgets)
    {
        widgets.xForEach(item =>
        {
            var exists = Widgets.FirstOrDefault(m => m.WigetName == item.WigetName);
            exists.xIfNotEmpty(() => throw new DuplicateNameException("Widget is exists"));
            Widgets.Add(item);
        });
    }

    public WidgetGroup AddControl(int index, WidgetBase widgetBase)
    {
        var exists = Widgets.FirstOrDefault(m => m.WigetName == widgetBase.WigetName);
        exists.xIfNotEmpty(() => throw new DuplicateNameException("Widget is exists"));
        this.Widgets.Add(widgetBase);
        return this;
    }

    public WidgetGroup InsertControl(int index, WidgetBase widgetBase)
    {
        var exists = Widgets.FirstOrDefault(m => m.WigetName == widgetBase.WigetName);
        exists.xIfNotEmpty(() => throw new DuplicateNameException("Widget is exists"));
        this.Widgets.Insert(index, widgetBase);
        return this;
    }

    public WidgetGroup RemoveControl(WidgetBase widgetBase)
    {
        var exists = Widgets.FirstOrDefault(m => m.WigetName == widgetBase.WigetName);
        exists.xIfEmpty(() => throw new KeyNotFoundException("Widget not exists"));
        this.Widgets.Remove(exists);

        return this;
    }
}