using eXtensionSharp;

namespace JUIControls;

public sealed class ENUM_FORM_TYPE : XEnumBase<ENUM_FORM_TYPE>
{
    /// <summary>
    /// 입력형
    /// </summary>
    public static readonly ENUM_FORM_TYPE INPUT = Define("INPUT");
    /// <summary>
    /// 검색형
    /// </summary>
    public static readonly ENUM_FORM_TYPE SEARCH = Define("SEARCH");
    /// <summary>
    /// 리스트형
    /// </summary>
    public static readonly ENUM_FORM_TYPE LIST = Define("LIST");
    /// <summary>
    /// 출력형
    /// </summary>
    public static readonly ENUM_FORM_TYPE PRINT = Define("PRINT");
}