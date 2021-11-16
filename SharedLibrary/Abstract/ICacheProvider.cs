using System;
using SharedLibrary.Infrastructure;

namespace SharedLibrary.Abstract
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