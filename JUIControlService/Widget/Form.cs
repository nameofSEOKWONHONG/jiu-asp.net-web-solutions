using System.Collections.Generic;

namespace JUIControlService.Widget;

public class Form
{
    public string MenuCode { get; set; }
    public string MenuName { get; set; }
    /// <summary>
    /// 폼 타입
    /// </summary>
    public ENUM_FORM_TYPE FormType { get; set; }
    public FormHeader Header { get; set; }
    public FormBody Body { get; set; }
    public FormFooter Footer { get; set; }
}

public class FormHeader
{
    public List<WidgetSectionGroup> WidgetSectionGroups { get; set; }
}

public class FormBody
{
    public List<WidgetSectionGroup> WidgetSectionGroups { get; set; }
}

public class FormFooter
{
    public List<WidgetSectionGroup> WidgetSectionGroups { get; set; }
}