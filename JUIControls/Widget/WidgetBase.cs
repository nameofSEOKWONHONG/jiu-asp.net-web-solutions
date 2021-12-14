using System.Collections.Generic;

namespace JUIControls.Widget;

/// <summary>
/// 
/// </summary>
public class WidgetBase 
{
    /// <summary>
    /// 컨트롤 인덱스 (순번)
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 컨트롤명
    /// </summary>
    public string WigetName { get; set; }
    /// <summary>
    /// 컨트롤 타입
    /// </summary>
    public ENUM_WIDGET_TYPE WidgetType { get; set; }
    /// <summary>
    /// 값
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 값 리스트 옵션 (Key 디스플레이, Value 값)
    /// 리스트 및 autocomplete에 사용
    /// </summary>
    public Dictionary<string, object> ValueOptions { get; set; }

    public bool NewLine { get; set; } = false;
}