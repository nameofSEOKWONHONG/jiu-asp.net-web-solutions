using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Infrastructure.Cache
{
    public abstract class CacheProviderBase
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        public CacheProviderBase(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        
        protected string CreateCacheKey()
        {
            var sb = new XStringBuilder();
            HttpContextAccessor.HttpContext.Request.Query.xForEach(item =>
            {
                sb.Append($"{item.Key}:{item.Value}");
            });
            HttpContextAccessor.HttpContext.GetRouteData().Values.xForEach(item =>
            {
                sb.Append($"{item.Key}:{item.Value}");
            });
            sb.Release(out string key);
            return key.xGetHashCode();
        }

        protected string CreateCacheKey(string key)
        {
            return CreateCacheKey(new[] {key});
        }

        protected string CreateCacheKey(string[] keys)
        {
            var sb = new XStringBuilder(keys.Length);
            keys.xForEach(item =>
            {
                sb.Append(item);
            });
            sb.Release(out string combinedKey);
            return combinedKey.xGetHashCode();
        }
    }
}