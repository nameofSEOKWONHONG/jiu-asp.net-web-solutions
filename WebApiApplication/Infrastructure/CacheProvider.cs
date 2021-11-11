using System;
using System.Linq;
using System.Text;
using eXtensionSharp;
using Microsoft.Extensions.Caching.Memory;

namespace WebApiApplication.Infrastructure
{
    public interface ICacheProvider
    {
        T GetCache<T>(string key);
        T GetCache<T>(MemoryCacheData<T> data);
        void SetCache<T>(MemoryCacheData<T> data);
        void SetCache<T>(string key, T value);
        void SetCache<T>(string key, T value, DateTimeOffset? duration);
        void ClearCache(string key);
        void ClearCache<T>(MemoryCacheData<T> data);
    }

    public class MemoryCacheData<T>
    {
        public string[] Keys { get; set; }
        public MemoryCacheEntryOptions Options { get; set; }
        public T Data { get; set; }
    }
    
    public class CacheProvider : ICacheProvider
    {
        private readonly int _expireSeconds = 10; // 10 Seconds
        
        private readonly IMemoryCache _cache;

        public CacheProvider(IMemoryCache cache, int expireSeconds = 10)
        {
            _cache = cache;
            _expireSeconds = expireSeconds;
        }

        public CacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public T GetCache<T>(string key)
        {
            if(_cache.TryGetValue<T>(key, out T value))
            {
                return value;
            }

            return default;
        }

        public T GetCache<T>(MemoryCacheData<T> data)
        {
            var sum = data.Keys.Select(m => m.Length).Sum();
            var sb = new StringBuilder(sum);
            data.Keys.xForEach(item =>
            {
                sb.Append(item);
            });
            var hashedKey = sb.ToString().xToHash();
            return GetCache<T>(hashedKey);
        }

        public void SetCache<T>(MemoryCacheData<T> data)
        {
            var sum = data.Keys.Select(m => m.Length).Sum();
            var sb = new StringBuilder(sum);
            data.Keys.xForEach(item =>
            {
                sb.Append(item);
            });
            var hashedKey = sb.ToString().xToHash();
            _cache.Set<T>(hashedKey, data.Data, data.Options);
        }
        
        public void SetCache<T>(string key, T value)
        {
            if (_expireSeconds > 0)
                SetCache<T>(key, value, DateTimeOffset.Now.AddSeconds(_expireSeconds));
            else
                SetCache<T>(key, value, null);
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration = null)
        {
            if(duration != null)
                _cache.Set<T>(key, value);
            else 
                _cache.Set<T>(key, value, duration.Value);
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }

        public void ClearCache<T>(MemoryCacheData<T> data)
        {
            var sum = data.Keys.Select(m => m.Length).Sum();
            var sb = new StringBuilder(sum);
            data.Keys.xForEach(item =>
            {
                sb.Append(item);
            });
            var hashedKey = sb.ToString().xToHash();
            ClearCache(hashedKey);
        }
    }
}