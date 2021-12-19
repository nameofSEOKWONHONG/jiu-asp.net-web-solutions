namespace Application.Infrastructure.Cache
{
    /// <summary>
    /// 캐시 옵션
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheOptions<T>
    {
        /// <summary>
        /// 키로 지정될 목록
        /// </summary>
        public string[] Keys { get; set; }

        /// <summary>
        /// 추가 키로 지정될 목록
        /// </summary>
        public string[] AdditionalKeys { get; set; }

        /// <summary>
        /// 캐시옵션
        /// </summary>
        public CacheEntryOptions Options { get; set; }
        
        /// <summary>
        /// 캐시 데이타
        /// </summary>
        public T Data { get; set; }
    }

    public class CacheEntryOptions
    {
        /// <summary>
        /// 만료 시간
        /// </summary>
        public int? ExpireTimeout { get; set; } = 10;
    }
    
}