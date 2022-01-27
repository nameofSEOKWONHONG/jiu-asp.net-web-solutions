using System;
using System.IO;
using System.Threading.Tasks;
using Application.Context;
using Application.Infrastructure.Cache;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using Jint;
using Microsoft.ClearScript;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    public class IndexController : ApiControllerBase<IndexController>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly JIUDbContext _dbContext;
        private readonly SharpScriptLoader _sharpScriptLoader;
        private readonly JsScriptLoader _jsScriptLoader;
        private readonly PyScriptLoader _pyScriptLoader;
        private readonly JIntScriptLoader _jIntScriptLoader;
        
        public IndexController(CacheProviderResolver resolver, 
            JIUDbContext dbContext, 
            SharpScriptLoader sharpScriptLoader,
            JsScriptLoader jsScriptLoader,
            PyScriptLoader pyScriptLoader,
            JIntScriptLoader jIntScriptLoader)
        {
            _cacheProvider = resolver(ENUM_CACHE_TYPE.FASTER);
            _dbContext = dbContext;
            _sharpScriptLoader = sharpScriptLoader;
            _jsScriptLoader = jsScriptLoader;
            _pyScriptLoader = pyScriptLoader;
            _jIntScriptLoader = jIntScriptLoader;
        }
        
        [HttpGet("index")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            #region [script execute sample]

            var csResult = _sharpScriptLoader.Create("./ScriptFiles/cs/HelloWorldScript.cs")
                .Execute<JIUDbContext, string, string>(_dbContext, "Run as CsScript",
                    new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #region [scope sample]

            var scriptResult2 = _sharpScriptLoader.Create("./ScriptFiles/cs/HelloWorldScript.cs")
                .Execute<JIUDbContext, string, string>(_dbContext, "Run as CsScript",
                    new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #endregion


            #endregion

            #region [cs-script native execute sample]

            // var nativeResult = _csScriptLoader.Create("./ScriptFiles/cs/HelloWorldScript.cs")
            //     .Execute<HelloWorldScript, JIUDbContext, string, string>(_dbContext, "Run as CsScript",
            //         new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #endregion

            #region [clear script sample]
            var modulePath = Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\csscript\\modules");
            var mainJsPath = "ScriptFiles\\js\\csscript\\main.js";
            var jsResult = string.Empty;
            _jsScriptLoader.Create(mainJsPath, modulePath)
                .Execute<string>("test",
                    engine =>
                    {
                        engine.AddHostType("Console", typeof(Console));
                        var typeCollection = new HostTypeCollection("mscorlib", "System", "System.Core");
                        // typeCollection.AddAssembly(typeof(MyType1).Assembly);
                        // typeCollection.AddAssembly(typeof(MyType2).Assembly);
                        engine.AddHostObject("clr", typeCollection);
                    },
                    result =>
                    {
                        jsResult = result.xValue<string>();
                    });
            #endregion

            #region [ironpython sample]

            var pyResult = string.Empty;
            var pypath = Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\py\\sample.py");
            _pyScriptLoader.Create(pypath).Execute(set =>
            {
                set.SetVariable("test", "hello world!");
            }, get =>
            {
                if (get.TryGetVariable<float>("sum", out float result))
                {
                    pyResult = result.ToString("F6");
                }

                if (get.TryGetVariable<string>("test", out string str))
                {
                    pyResult = $"{pyResult} and {str}";
                }
            });

            #endregion

            #region [JintScript sample]

            var jintResult = string.Empty;
            _jIntScriptLoader.Create("ScriptFiles/js/jint/sample.js").Execute(
                engine =>
                {
                    engine.SetValue("requestTxt", "I'm JInt");
                }, engine =>
                {
                    jintResult = engine.GetValue("result").AsString();
                });

            var tsResult = string.Empty;
            _jIntScriptLoader.Create("ScriptFiles\\js\\jint\\sample1.ts")
                .Execute(engine =>
                    {
                        
                    }, engine =>
                    {
                        tsResult = engine.GetValue("result").AsString();
                    });

            #endregion
            
            //var key = "IndexMessage";
            //var result = _cacheProvider.GetCache<string>(key);
            var result = string.Empty;
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
                    sb.AppendLine($"cs-script : {csResult}");
                    sb.AppendLine($"js-script : {jsResult}");
                    sb.AppendLine($"py-script : {pyResult}");
                    sb.AppendLine($"jint-script : {jintResult}");
                    sb.AppendLine($"ts-script : {tsResult}");
                    sb.Release(out string str);
                    return str;
                });
                //_cacheProvider.SetCache<string>(key, result);
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