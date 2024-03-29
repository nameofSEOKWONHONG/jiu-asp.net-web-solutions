﻿using System;
using eXtensionSharp;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Infrastructure.Cache
{
    /// <summary>
    /// litedb cache
    /// TODO : 인터페이스 구현 및 만료 구현되어야 함.
    /// </summary>
    public class LiteDbCacheProvider : CacheProviderBase, ICacheProvider
    {
        private readonly LiteDatabase _liteDatabase;
        public LiteDbCacheProvider(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            var connectionString = new ConnectionString()
            {
                Filename = configuration["LiteDatabaseConfiguration:Uri"],
                Connection = ConnectionType.Direct
            };
            this._liteDatabase = new LiteDatabase(connectionString);
        }

        public int Count
        {
            get => throw new NotImplementedException();
        }
        
        public T GetCache<T>()
        {
            var collection = _liteDatabase.GetCollection<T>();
            return collection.FindOne(this.CreateCacheKey());
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
            throw new NotImplementedException();
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

        public void Reset<T>()
        {
            var collection = _liteDatabase.GetCollection<T>();
            collection.DeleteAll();
        }

        public void Dispose()
        {
            if(this._liteDatabase.xIsNotEmpty())
                this._liteDatabase.Dispose();
        }
    }
}