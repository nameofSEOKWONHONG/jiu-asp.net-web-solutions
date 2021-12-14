﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using eXtensionSharp;

namespace JUIControls.Widget;

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

    public List<WidgetBase> Widgets { get; set; }
}