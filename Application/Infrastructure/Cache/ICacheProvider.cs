using System;
using Application.Infrastructure;

namespace Application.Infrastructure.Cache
{
    public interface ICacheProvider
    {
        int Count { get; }
        T GetCache<T>();
        T GetCache<T>(string key);
        T GetCache<T>(CacheOptions<T> options);
        void SetCache<T>(T value, int expireTimeout = 10);
        void SetCache<T>(string key, T value, int expireTimeout = 10);
        void SetCache<T>(string key, T value, DateTimeOffset? duration);        
        void SetCache<T>(CacheOptions<T> options);
        void RemoveCache(string key);
        void RemoveCache<T>(CacheOptions<T> options);
        void Reset();
        void Flush();
    }
}