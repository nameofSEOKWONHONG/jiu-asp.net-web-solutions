using System.Collections.Generic;
using System.Data;
using System.Linq;
using eXtensionSharp;

namespace JUIControls.Widget;

/// <summary>
/// 위젯 그룹
/// </summary>
public class WidgetGroup
{
    /// <summary>
    /// 그룹 Name (빈값일 경우 외곽선 및 명칭 표기가 되지 않는다.)
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// 다음 컨트롤 신규 라인 여부
    /// </summary>
    public bool NewLine { get; set; } = false;

    public List<WidgetBase> Widgets { get; set; }
}