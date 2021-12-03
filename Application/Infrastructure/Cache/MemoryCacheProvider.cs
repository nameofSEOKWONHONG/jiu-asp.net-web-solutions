using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using eXtensionSharp;
using Microsoft.Extensions.Caching.Memory;
using Application.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Application.Infrastructure.Cache
{   
    public class MemoryCacheProvider : CacheProviderBase, ICacheProvider
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public int Count => (_cache as MemoryCache).Count;
        
        public MemoryCacheProvider(IMemoryCache cache, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public T GetCache<T>()
        {
            return GetCache<T>(CreateCacheKey());
        }
        
        public T GetCache<T>(string key)
        {
            if(_cache.TryGetValue<T>(key, out T value))
            {
                return value;
            }

            return default;
        }

        public T GetCache<T>(CacheOptions<T> options)
        {
            var sum = options.Keys.Select(m => m.Length).Sum();
            var sb = new StringBuilder(sum);
            options.Keys.xForEach(item =>
            {
                sb.Append(item);
            });
            var hashedKey = sb.ToString().xToHash();
            return GetCache<T>(hashedKey);
        }

        public void SetCache<T>(T value, int? expireTimeout = null)
        {
            SetCache<T>(CreateCacheKey(), value, expireTimeout);
        }

        public void SetCache<T>(string key, T value, int? expireTimeout = null)
        {
            var options = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = expireTimeout.HasValue ? DateTimeOffset.Now.AddSeconds(expireTimeout.Value) : null,
                ExpirationTokens = { new CancellationChangeToken(_resetCacheToken.Token) }
            };
            _cache.Set<T>(key, value, options);
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration = null)
        {
            var options = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = duration,
                ExpirationTokens = { new CancellationChangeToken(_resetCacheToken.Token) }
            };
            _cache.Set<T>(key, value, options);
        }
        
        public void SetCache<T>(CacheOptions<T> options)
        {
            var hashedKey = this.CreateCacheKey(options.Keys);
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = options.Options.ExpireTimeout.HasValue ? DateTimeOffset.Now.AddSeconds(options.Options.ExpireTimeout.Value) : null,
                ExpirationTokens = { new CancellationChangeToken(_resetCacheToken.Token) }
            };            
            
            _cache.Set<T>(hashedKey, options.Data, memoryCacheEntryOptions);
        }        

        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveCache<T>(CacheOptions<T> options)
        {
            var hashedKey = this.CreateCacheKey(options.Keys);
            RemoveCache(hashedKey);
        }
        
        public void Reset()
        {
            ResetImpl();
        }

        private void ResetImpl()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();            
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }
    }
}