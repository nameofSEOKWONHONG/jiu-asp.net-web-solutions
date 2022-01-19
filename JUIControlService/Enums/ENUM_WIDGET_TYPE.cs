using eXtensionSharp;

namespace JUIControlService;

/// <summary>
/// 위젯 개별 구성 요소
/// </summary>
public sealed class ENUM_WIDGET_TYPE : XEnumBase<ENUM_WIDGET_TYPE>
{
    public static readonly ENUM_WIDGET_TYPE BUTTON = Define("BUTTON");
    public static readonly ENUM_WIDGET_TYPE LABEL = Define("LABEL");
    public static readonly ENUM_WIDGET_TYPE INPUT = Define("INPUT");
    public static readonly ENUM_WIDGET_TYPE DATAGRID = Define("DATAGRID");
    public static readonly ENUM_WIDGET_TYPE CHECKBOX = Define("CHECKBOX");
    public static readonly ENUM_WIDGET_TYPE DATETIME_PICKER = Define("DATETIME_PICKER");
}