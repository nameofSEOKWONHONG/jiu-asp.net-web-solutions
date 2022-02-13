using System;
using System.IO;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RocksDbSharp;

namespace Application.Infrastructure.Cache;

public class RocksDbCacheProvider : CacheProviderBase, ICacheProvider
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "rockdbcache");
    private readonly RocksDb _dbInstance;
    public RocksDbCacheProvider(IConfiguration configuration, 
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        var options = new DbOptions()
            .SetCreateIfMissing(true);
        _dbInstance = RocksDb.Open(options, _filePath);
    }

    public int Count { get; }
    public T GetCache<T>()
    {
        var cached = _dbInstance.Get(this.CreateCacheKey());
        return cached.xToEntity<T>();
    }

    public T GetCache<T>(CacheOptions<T> options)
    {
        throw new NotImplementedException();
    }

    public T GetCache<T>(string key)
    {
        throw new NotImplementedException();
    }

    public void SetCache<T>(T value, int? expireTimeout)
    {
        _dbInstance.Put(this.CreateCacheKey(), value.xToJson());
    }

    public void SetCache<T>(string key, T value, int? expireTimeout)
    {
        throw new NotImplementedException();
    }

    public void SetCache<T>(string key, T value, DateTimeOffset? expireTimeout)
    {
        throw new NotImplementedException();
    }

    public void SetCache<T>(CacheOptions<T> options)
    {
        throw new NotImplementedException();
    }

    public void RemoveCache(string key)
    {
        _dbInstance.Remove(key);
    }

    public void RemoveCache<T>(CacheOptions<T> options)
    {
        var hashedKey = this.CreateCacheKey(options.Keys);
        _dbInstance.Remove(hashedKey);
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
        if(this._dbInstance.xIsNotEmpty())
            this._dbInstance.Dispose();
    }
}