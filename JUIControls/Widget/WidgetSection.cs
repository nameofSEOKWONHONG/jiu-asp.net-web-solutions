using System.Collections.Generic;

namespace JUIControls.Widget;

public class WidgetSection
{
    /// <summary>
    /// 마스터 유뮤, 마스터는 무조건 왼쪽에 배치한다.
    /// </summary>
    public bool IsMaster { get; set; } = false;
    
    public bool NewLine { get; set; } = false;
    
    public List<WidgetGroup> WidgetGroups { get; set; }
}