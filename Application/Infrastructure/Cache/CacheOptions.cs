using eXtensionSharp;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Infrastructure.Cache
{
    public class CacheOptions<T>
    {
        public string[] Keys
        {
            get;
            set;
        }
        
        public string[] AdditionalKeys { get; set; }

        public CacheEntryOptions Options { get; set; }
        public T Data { get; set; }
    }

    public class CacheEntryOptions
    {
        public int? ExpireTimeout { get; set; } = 0;
    }
    
}