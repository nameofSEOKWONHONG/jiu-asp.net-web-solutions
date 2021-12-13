using System.Collections.Generic;

namespace JUIControls;

/// <summary>
/// Widget Binder Interface
/// </summary>
public interface IWidgetBinder
{
    /// <summary>
    /// 구현
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    (string value, Dictionary<string, string> valueOptions) Bind(Dictionary<string, string> options);
}