using System;
using Application.Abstract;

namespace Application.Infrastructure.Cache
{
    public class RedisCacheProvider : ICacheProvider
    {
        public T GetCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(MemoryCacheData<T> data)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(MemoryCacheData<T> data)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration)
        {
            throw new NotImplementedException();
        }

        public void ClearCache(string key)
        {
            throw new NotImplementedException();
        }

        public void ClearCache<T>(MemoryCacheData<T> data)
        {
            throw new NotImplementedException();
        }
    }
}