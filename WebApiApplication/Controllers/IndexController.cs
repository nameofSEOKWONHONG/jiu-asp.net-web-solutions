using System.Text;
using System.Threading.Tasks;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using WebApiApplication.Services;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    public class IndexController : ApiControllerBase<IndexController>
    {
        private readonly ICacheProvider _cacheProvider;
        public IndexController(CacheProviderResolver resolver)
        {
            _cacheProvider = resolver(ENUM_CACHE_TYPE.FASTER);
        }
        [HttpGet]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var key = "IndexMessage";
            var result = _cacheProvider.GetCache<string>(key);
            if (result.xIsEmpty())
            {
                result = await Task.Factory.StartNew(() =>
                {
                    var sb = new XStringBuilder();
                    sb.AppendLine("Hello, The purpose of this project is to create a template similar to \"typescript-express-starter\"");
                    sb.AppendLine("(https://github.com/ljlm0402/typescript-express-starter)");
                    sb.AppendLine("git clone [project site] [your project name]");
                    sb.AppendLine("cd [your project name]");
                    sb.AppendLine("dotnet restore");
                    sb.AppendLine("dotnet run or dotnet watch run");
                    sb.Release(out string str);
                    return str;
                });
                _cacheProvider.SetCache<string>(key, result);
            }
            
            return Ok(result);
        }
    }
}