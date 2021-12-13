using System.Collections.Generic;

namespace JUIControls;

/// <summary>
/// 폼 전체 구성
/// </summary>
public class Section
{
    public string MenuCode { get; set; }
    public string MenuName { get; set; }
    /// <summary>
    /// 섹션 타입
    /// </summary>
    public ENUM_SECTION_TYPE SectionType { get; set; }
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