using System.Collections.Generic;

namespace JUIControls;

public class JUIControl
{
    /// <summary>
    /// 컨트롤 인덱스 (순번)
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 컨트롤명
    /// </summary>
    public string ControlName { get; set; }
    /// <summary>
    /// Bind명
    /// </summary>
    public string BinderName { get; set; }
    /// <summary>
    /// 컨트롤 타입
    /// </summary>
    public ENUM_CONTROL_TYPE ControlType { get; set; }
    /// <summary>
    /// 값
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 값 리스트 옵션 (Key 디스플레이, Value 값)
    /// 리스트 및 autocomplete에 사용
    /// </summary>
    public Dictionary<string, string> ValueOptions { get; set; }
}