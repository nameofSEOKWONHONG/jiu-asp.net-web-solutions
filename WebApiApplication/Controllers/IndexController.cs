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
            _cacheProvider = resolver(ENUM_CACHE_TYPE.REDIS);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = _cacheProvider.GetCache<string>("test");
            if (result.xIsEmpty())
            {
                result = await Task.Factory.StartNew(() =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Hello, The purpose of this project is to create a template similar to \"typescript-express-starter\"");
                    sb.AppendLine("(https://github.com/ljlm0402/typescript-express-starter)");
                    sb.AppendLine("git clone [project site] [your project name]");
                    sb.AppendLine("cd [your project name]");
                    sb.AppendLine("dotnet restore");
                    sb.AppendLine("dotnet run or dotnet watch run");
                    return sb.ToString();
                });
                _cacheProvider.SetCache<string>("test", result);
            }
            
            return Ok(result);
        }
    }
}