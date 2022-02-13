namespace Application.Infrastructure.Cache.MSFaster;

/// <summary>
/// User context to measure latency and/or check read result
/// </summary>
public struct FasterCacheContext
{
    public int type;
    public long ticks;
}