using System;
using System.Linq;
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
            return GetCacheImpl<T>(string.Empty);
        }
        
        public T GetCache<T>(CacheOptions<T> options)
        {
            var key = string.Join("", options.Keys);
            return GetCacheImpl<T>(key);
        }

        public T GetCache<T>(string key)
        {
            return GetCacheImpl<T>(key);
        }

        private T GetCacheImpl<T>(string key)
        {
            string hashedKey = string.Empty;
            key.xIfEmpty(() => hashedKey = this.CreateCacheKey());
            key.xIfNotEmpty(() => hashedKey = key.xToHash());
            var value = _distributedCache.GetString(hashedKey);
            if(value.xIsNotEmpty())
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }
        
        public void SetCache<T>(T value, int expireTimeout = 10)
        {
            var hashedKey = this.CreateCacheKey();
            _distributedCache.Set(hashedKey, value.xToBytes());
        }

        public void SetCache<T>(string key, T value, int expireTimeout = 10)
        {
            var hashedKey = this.CreateCacheKey(key);
            _distributedCache.Set(hashedKey, value.xToBytes());
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration)
        {
            var hashedKey = this.CreateCacheKey(key);
            _distributedCache.Set(hashedKey, value.xToBytes());
        }

        public void SetCache<T>(CacheOptions<T> options)
        {
            var hashedKey = this.CreateCacheKey(options.Keys);
            _distributedCache.Set(hashedKey, options.Data.xToBytes());
        }

        public void RemoveCache(string key)
        {
            var hashedKey = this.CreateCacheKey(key);
            _distributedCache.Remove(hashedKey);
        }

        public void RemoveCache<T>(CacheOptions<T> options)
        {
            var hashedKey = this.CreateCacheKey(options.Keys);
            _distributedCache.Remove(hashedKey);
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