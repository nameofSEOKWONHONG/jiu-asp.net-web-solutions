using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums;

/// <summary>
/// 캐시 DB 타입
/// </summary>
[TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_CACHE_TYPE>))]
[JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_CACHE_TYPE>))]
public class ENUM_CACHE_TYPE : XEnumBase<ENUM_CACHE_TYPE>
{
    /// <summary>
    /// 인메모리
    /// </summary>
    public static readonly ENUM_CACHE_TYPE MEMORY = Define("MEMORY");
    /// <summary>
    /// 레디스
    /// </summary>
    public static readonly ENUM_CACHE_TYPE REDIS  = Define("REDIS");
    /// <summary>
    /// litedb (로컬db)
    /// </summary>
    public static readonly ENUM_CACHE_TYPE LITEDB = Define("LITEDB");
    /// <summary>
    /// faster (로컬db)
    /// </summary>
    public static readonly ENUM_CACHE_TYPE FASTER = Define("FASTER");
    /// <summary>
    /// rocksdb (로컬db)
    /// </summary>
    public static readonly ENUM_CACHE_TYPE ROCKS = Define("ROCKS");
}
