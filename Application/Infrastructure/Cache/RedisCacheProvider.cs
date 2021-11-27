using System;
using System.Text.Json;
using Application.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace Application.Infrastructure.Cache
{
    public class RedisCacheProvider : CacheProviderBase, ICacheProvider
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCacheProvider(IHttpContextAccessor httpContextAccessor,
            IDistributedCache distributedCache) : base(httpContextAccessor)
        {
            _distributedCache = distributedCache;
        }

        public int Count
        {
            get => throw new NotSupportedException();
        }

        public T GetCache<T>()
        {
            var key = CreateCacheKey();
            return GetCache<T>(key);
        }
        
        public T GetCache<T>(CacheOptions<T> options)
        {
            var hashedKey = this.CreateCacheKey(options.Keys);
            return GetCache<T>(hashedKey);
        }

        public T GetCache<T>(string key)
        {
            var value = _distributedCache.GetString(key);
            return JsonSerializer.Deserialize<T>(value);
        }

        public void SetCache<T>(T value, int expireTimeout = 10)
        {
            
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

        public void Flush()
        {
            throw new NotImplementedException();
        }
    }
}