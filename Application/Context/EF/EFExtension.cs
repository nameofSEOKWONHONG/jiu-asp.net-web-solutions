using Application.Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Context.EF;

public static class EntityFrameworkExtension
{
    public static void SetCache<T>(this DbContext dbContext, ICacheProvider cacheProvider, CacheOptions<T> cacheOptions)
    {
        cacheProvider.SetCache(cacheOptions);
    }

    public static TR GetCache<T, TR>(this DbContext dbContext, ICacheProvider cacheProvider, CacheOptions<T> cacheOptions)
    {
        return cacheProvider.GetCache<TR>();
    }
}