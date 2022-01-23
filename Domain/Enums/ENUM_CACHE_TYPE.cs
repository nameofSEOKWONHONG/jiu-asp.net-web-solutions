using eXtensionSharp;

namespace Domain.Enums;

public class ENUM_CACHE_TYPE : XEnumBase<ENUM_CACHE_TYPE>
{
    public static readonly ENUM_CACHE_TYPE MEMORY = Define("MEMORY");
    public static readonly ENUM_CACHE_TYPE REDIS  = Define("REDIS");
    public static readonly ENUM_CACHE_TYPE LITEDB = Define("LITEDB");
    public static readonly ENUM_CACHE_TYPE FASTER = Define("FASTER");
    public static readonly ENUM_CACHE_TYPE ROCKS = Define("ROCKS");
}