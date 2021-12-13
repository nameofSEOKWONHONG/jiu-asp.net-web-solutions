using eXtensionSharp;

namespace JUIControls;

public sealed class ENUM_SECTION_TYPE : XEnumBase<ENUM_SECTION_TYPE>
{
    /// <summary>
    /// 입력형
    /// </summary>
    public static readonly ENUM_SECTION_TYPE INPUT = Define("INPUT");
    /// <summary>
    /// 검색형
    /// </summary>
    public static readonly ENUM_SECTION_TYPE SEARCH = Define("SEARCH");
    /// <summary>
    /// 리스트형
    /// </summary>
    public static readonly ENUM_SECTION_TYPE LIST = Define("LIST");
    /// <summary>
    /// 출력형
    /// </summary>
    public static readonly ENUM_SECTION_TYPE PRINT = Define("PRINT");
}