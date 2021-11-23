using System;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure.Cache
{
    public class LiteDbCacheProvider : CacheProviderBase, ICacheProvider
    {
        public LiteDbCacheProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public int Count { get; }

        public T GetCache<T>()
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(T value, int expireTimeout = 10)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(string key, T value, int expireTimeout = 10)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}