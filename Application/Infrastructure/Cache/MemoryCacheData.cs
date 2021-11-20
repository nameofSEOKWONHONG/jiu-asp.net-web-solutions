using Microsoft.Extensions.Caching.Memory;

namespace Application.Infrastructure.Cache
{
    public class MemoryCacheData<T>
    {
        public string[] Keys { get; set; }
        public MemoryCacheEntryOptions Options { get; set; }
        public T Data { get; set; }
    }
}