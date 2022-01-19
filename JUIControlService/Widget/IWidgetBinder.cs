using System;
using System.Collections.Generic;

namespace JUIControlService.Widget;

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
    (string value, Dictionary<string, Object> valueOptions) Bind(Dictionary<string, string> options);
}