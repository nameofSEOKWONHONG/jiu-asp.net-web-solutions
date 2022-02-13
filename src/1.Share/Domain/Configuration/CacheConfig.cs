using Domain.Enums;

namespace Domain.Configuration;

public class CacheConfig
{
    public bool IsCacheReset { get; set; }
    public ENUM_CACHE_TYPE[] ResetCacheTypes { get; set; }
}