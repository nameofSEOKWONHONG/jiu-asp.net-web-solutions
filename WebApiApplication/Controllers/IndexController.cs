using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Context;
using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using eXtensionSharp;
using FASTER.core;
using Infrastructure.Abstract;
using WebApiApplication.Services;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    public class IndexController : ApiControllerBase<IndexController>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly JIUDbContext _dbContext;
        private readonly SharpScriptLoader _sharpScriptLoader;
        private readonly JsScriptLoader _jsScriptLoader;
        
        public IndexController(CacheProviderResolver resolver, 
            JIUDbContext dbContext, 
            SharpScriptLoader sharpScriptLoader,
            JsScriptLoader jsScriptLoader)
        {
            _cacheProvider = resolver(ENUM_CACHE_TYPE.FASTER);
            _dbContext = dbContext;
            _sharpScriptLoader = sharpScriptLoader;
            _jsScriptLoader = jsScriptLoader;
        }
        
        [HttpGet("index")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            #region [script execute sample]

            var scriptResult = _sharpScriptLoader.Create("./CsScriptFiles/HelloWorldScript.cs")
                .Execute<JIUDbContext, string, string>(_dbContext, "Run as CsScript",
                    new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #region [scope sample]

            var scriptResult2 = _sharpScriptLoader.Create("./CsScriptFiles/HelloWorldScript.cs")
                .Execute<JIUDbContext, string, string>(_dbContext, "Run as CsScript",
                    new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #endregion


            #endregion

            #region [native execute sample]

            // var nativeResult = _csScriptLoader.Create("./CsScriptFiles/HelloWorldScript.cs")
            //     .Execute<HelloWorldScript, JIUDbContext, string, string>(_dbContext, "Run as CsScript",
            //         new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #endregion

            #region [clear script sample]
            var modulePath = Path.Combine(AppContext.BaseDirectory, "CsScriptFiles\\js\\modules");
            var mainJsPath = "./CsScriptFiles/js/main.js";
            var jsResult = string.Empty;
            _jsScriptLoader.Create(mainJsPath, modulePath)
                .Execute<string>("test", o =>
                {
                    jsResult = o.xValue<string>();
                });
            #endregion
            
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
                    sb.AppendLine($"cs-script : {scriptResult}");
                    sb.AppendLine($"clear-script : {jsResult}");
                    sb.Release(out string str);
                    return str;
                });
                _cacheProvider.SetCache<string>(key, result);
            }
            
            return Ok(result);
        }

        [HttpPost("sample")]
        public IActionResult Sample(SampleDto dto)
        {
            return Ok(dto);
        } 
        
        /// <summary>
        /// not working
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpPost("sample2")]
        [Obsolete("don't use case")]
        public IActionResult Sample2([FromBody]ENUM_ROLE_TYPE str)
        {
            return Ok(str);
        }
    }

    public class SampleDto
    {
        public int Id { get; set; }
        public ENUM_ROLE_TYPE RoleType { get; set; }
    }
}