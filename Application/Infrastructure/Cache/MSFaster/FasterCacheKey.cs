using FASTER.core;

namespace Application.Infrastructure.Cache.MSFaster;

public struct FasterCacheKey : IFasterEqualityComparer<FasterCacheKey>
{
    public string key;

    public FasterCacheKey(string first)
    {
        key = first;
    }

    public long GetHashCode64(ref FasterCacheKey k)
    {
        return k.GetHashCode();
    }

    public bool Equals(ref FasterCacheKey k1, ref FasterCacheKey k2)
    {
        return k1.key == k2.key;
    }
}