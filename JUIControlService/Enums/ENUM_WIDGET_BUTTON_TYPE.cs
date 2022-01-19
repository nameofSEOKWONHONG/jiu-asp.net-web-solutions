using eXtensionSharp;

namespace JUIControlService;

/// <summary>
/// 버튼의 종류
/// </summary>
public sealed class ENUM_WIDGET_BUTTON_TYPE : XEnumBase<ENUM_WIDGET_BUTTON_TYPE>
{
    public static readonly ENUM_WIDGET_BUTTON_TYPE WidgetButton = Define("BUTTON");
    public static readonly ENUM_WIDGET_BUTTON_TYPE DropdownWidgetButton = Define("DROPDOWN_BUTTON");
    public static readonly ENUM_WIDGET_BUTTON_TYPE ImageWidgetButton = Define("IMAGE_BUTTON");
    public static readonly ENUM_WIDGET_BUTTON_TYPE ImageDropdownWidgetButton = Define("IMAGE_DROPDOWN_BUTTON");
}  