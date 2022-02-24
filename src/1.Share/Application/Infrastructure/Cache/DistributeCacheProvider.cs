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
    /// <summary>
    /// asp.net core 분산 캐싱
    /// support : mssql, redis, ncache
    /// <see href="https://docs.microsoft.com/ko-kr/aspnet/core/performance/caching/distributed?view=aspnetcore-6.0"/>
    /// </summary>
    public class DistributeCacheProvider : CacheProviderBase, ICacheProvider
    {
        private readonly IDistributedCache _distributedCache;
        public DistributeCacheProvider(IHttpContextAccessor httpContextAccessor,
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
            var hashedKey = string.Empty;
            key.xIfEmpty(() => hashedKey = this.CreateCacheKey());
            hashedKey = key.xIfNotEmpty<string>(() => key.xGetHashCode());
            var value = _distributedCache.GetString(hashedKey);
            if(value.xIsNotEmpty())
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }
        
        public void SetCache<T>(T value, int? expireTimeout = 10)
        {
            SetCacheImpl<T>(string.Empty, value, expireTimeout);
        }

        public void SetCache<T>(string key, T value, int? expireTimeout = 10)
        {
            SetCacheImpl<T>(key, value, expireTimeout);
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? expireTimeout)
        {
            SetCacheImpl<T>(key, value, expireTimeout.HasValue ? expireTimeout.Value.Second : null);
        }

        public void SetCache<T>(CacheOptions<T> options)
        {
            if (options.AdditionalKeys.xIsEmpty())
            {
                SetCacheImpl<T>(string.Join("", options.Keys), options.Data, options.Options.ExpireTimeout);    
            }
            else
            {
                var additionalKey = string.Join("|", options.AdditionalKeys);
                var key = string.Join("", options.Keys);
                var resultKey = $"{additionalKey}|{key.xGetHashCode()}";
                _distributedCache.Set(resultKey, options.Data.xToBytes(), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = options.Options.ExpireTimeout.HasValue ? TimeSpan.FromSeconds(options.Options.ExpireTimeout.Value) : null
                });
            }
        }

        private void SetCacheImpl<T>(string key, T value, int? expireTimeout = null)
        {
            var hashedKey = string.Empty;
            key.xIfEmpty(() => hashedKey = this.CreateCacheKey());
            hashedKey = key.xIfNotEmpty<string>(() => this.CreateCacheKey(key));
            _distributedCache.Set(hashedKey, value.xToBytes(), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expireTimeout.HasValue ? TimeSpan.FromSeconds(expireTimeout.Value) : null
            });
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

        public void Reset<T>()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //do nothing...
        }
    }
}