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
            return key.xToHash();
        }
    }
}