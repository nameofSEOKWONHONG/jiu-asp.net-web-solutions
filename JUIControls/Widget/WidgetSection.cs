using System.Collections.Generic;

namespace JUIControls.Widget;

/// <summary>
/// 폼 전체 구성
/// </summary>
public class WidgetSection
{
    
    /// <summary>
    /// 메뉴명, 검색창 등을 표시한다. 
    /// </summary>
    public List<WidgetGroup> Header { get; set; }
    /// <summary>
    /// 입력 항목, View 항목, 그리드 등을 표시한다.
    /// </summary>
    public List<WidgetGroup> Body { get; set; }
    /// <summary>
    /// 하단 기능, 버튼등을 표시한다.
    /// </summary>
    public List<WidgetGroup> Footer { get; set; }
}