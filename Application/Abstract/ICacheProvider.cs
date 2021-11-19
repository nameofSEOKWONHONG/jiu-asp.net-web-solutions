using System;
using Application.Infrastructure;

namespace Application.Abstract
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
}