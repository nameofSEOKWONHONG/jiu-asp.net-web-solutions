using eXtensionSharp;

namespace JUIControls;

public sealed class ENUM_CONTROL_TYPE : XEnumBase<ENUM_CONTROL_TYPE>
{
    public static readonly ENUM_CONTROL_TYPE BUTTON = Define("BUTTON");
    public static readonly ENUM_CONTROL_TYPE LABEL = Define("LABEL");
    public static readonly ENUM_CONTROL_TYPE INPUT = Define("INPUT");
}