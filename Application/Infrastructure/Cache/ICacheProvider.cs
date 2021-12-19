using System;
using Application.Infrastructure;

namespace Application.Infrastructure.Cache
{
    /// <summary>
    /// 캐시 제공자 인터페이스
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 캐시 카운트 조회 (Support. MemoryCacheProvider)
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 캐시조회
        /// HttpContext.Request 값에 의해 키가 설정됩니다. Get Method에만 사용하세요.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetCache<T>();
        T GetCache<T>(CacheOptions<T> options);
        T GetCache<T>(string key);
        /// <summary>
        /// 캐시설정
        /// HttpContext.Request 값에 의해 키가 설정됩니다. Get Method에만 사용하세요
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expireTimeout">만료시간(초)</param>
        /// <typeparam name="T"></typeparam>
        void SetCache<T>(T value, int? expireTimeout = 10);
        /// <summary>
        /// 캐시설정
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTimeout">만료시간(초)</param>
        /// <typeparam name="T"></typeparam>
        void SetCache<T>(string key, T value, int? expireTimeout = 10);
        /// <summary>
        /// 캐시설정
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTimeout">만료시간(초)</param>
        /// <typeparam name="T"></typeparam>
        void SetCache<T>(string key, T value, DateTimeOffset? expireTimeout);        
        void SetCache<T>(CacheOptions<T> options);
        void RemoveCache(string key);
        void RemoveCache<T>(CacheOptions<T> options);
        /// <summary>
        /// 캐시 초기화 합니다.(Support, MemoryCacheProvider)
        /// </summary>
        void Reset();
        /// <summary>
        /// 캐시 초기화 합니다.(Support, LiteDbCacheProvider)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Reset<T>();
    }
}