using eXtensionSharp;

namespace JUIControls;

public sealed class ENUM_BUTTON_TYPE : XEnumBase<ENUM_BUTTON_TYPE>
{
    public static readonly ENUM_BUTTON_TYPE BUTTON = Define("BUTTON");
    public static readonly ENUM_BUTTON_TYPE DROPDOWN_BUTTON = Define("DROPDOWN_BUTTON");
    public static readonly ENUM_BUTTON_TYPE IMAGE_BUTTON = Define("IMAGE_BUTTON");
    public static readonly ENUM_BUTTON_TYPE IMAGE_DROPDOWN_BUTTON = Define("IMAGE_DROPDOWN_BUTTON");
}