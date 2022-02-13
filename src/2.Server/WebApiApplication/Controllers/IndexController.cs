using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Context;
using Application.Infrastructure.Cache;
using Application.Script.ClearScript;
using Application.Script.SharpScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using eXtensionSharp;
using Hangfire;
using Infrastructure.Abstract;
using Jering.Javascript.NodeJS;
using Jint;
using Microsoft.ClearScript;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        private readonly NodeJSScriptLoader _nodeJsScriptLoader;
        private readonly INodeJSService _nodeJsService;
        
        public IndexController(CacheProviderResolver resolver, 
            JIUDbContext dbContext, 
            SharpScriptLoader sharpScriptLoader,
            JsScriptLoader jsScriptLoader,
            PyScriptLoader pyScriptLoader,
            JIntScriptLoader jIntScriptLoader,
            NodeJSScriptLoader nodeJsScriptLoader,
            INodeJSService nodeJsService)
        {
            _cacheProvider = resolver(ENUM_CACHE_TYPE.FASTER);
            _dbContext = dbContext;
            _sharpScriptLoader = sharpScriptLoader;
            _jsScriptLoader = jsScriptLoader;
            _pyScriptLoader = pyScriptLoader;
            _jIntScriptLoader = jIntScriptLoader;

            _nodeJsScriptLoader = nodeJsScriptLoader;
            _nodeJsService = nodeJsService;
        }
        
        [HttpGet("JavascriptSample")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> JavascriptSample()
        {
            #region [script execute sample]

            var csResult = _sharpScriptLoader.Create("HelloWorldScript.cs")
                .Execute<JIUDbContext, string, string>(_dbContext, "Run as CsScript",
                    new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

            #region [scope sample]

            var scriptResult2 = _sharpScriptLoader.Create("HelloWorldScript.cs")
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

            var modulePath = "ScriptFiles\\js\\csscript\\modules".xGetFileNameWithBaseDir();
            var mainJsPath = "main.js";
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
            var pyFile = "sample.py";
            _pyScriptLoader.Create(pyFile).Execute(set =>
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
            _jIntScriptLoader.Create("sample.js").Execute(
                engine =>
                {
                    engine.SetValue("requestTxt", "I'm JInt");
                }, engine =>
                {
                    jintResult = engine.GetValue("result").AsString();
                });

            var tsResult = string.Empty;
            _jIntScriptLoader.Create("sample1.ts")
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

        [HttpGet("NodeJSServiecSample")]
        public async Task<IActionResult> NodeJSServiecSample()
        {
            var result = string.Empty;

            _nodeJsScriptLoader.xThen(self =>
            {
                self.Create("index.js").Execute<string>(
                    () =>
                    {
                        var objs = new List<object>();
                        objs.Add(new NodeDataStructureSample() { A = "Hello", B = "World", C = "NodeJS" });
                        objs.Add(new NodeDataStructureSample()
                            { A = "It's Working??", B = "Working???", C = "Ye, It's Worked." });
                        return objs.ToArray();
                    },
                    s => { result = s; });
            }, e => _logger.LogInformation(e, e.Message));
            
            return Ok(result);
        }

        [HttpGet("xThenAsyncSample")]
        public async Task<IActionResult> xThenAsyncSample()
        {
            var result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                var response = await client.GetAsync("api/index/JavascriptSample?api-version=1");
                await response.xThenAsync(async self =>
                {
                    self.EnsureSuccessStatusCode();
                    result = await self.Content.ReadAsStringAsync();
                }, async e =>
                {
                    _logger.LogInformation(e, e.Message);
                    throw e;
                });
            }

            return Ok(result);
        }
        
        public class NodeDataStructureSample
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
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

        [HttpGet("HangFireSample")]
        public IActionResult HangFireSample()
        {
            var id = BackgroundJob.Enqueue(() => Console.WriteLine("I'm working at hangfire."));
            BackgroundJob.ContinueJobWith(id, () => Console.WriteLine("And I'm working continue."));
            return Ok(id);
        }
    }

    public class SampleDto
    {
        public int Id { get; set; }
        public ENUM_ROLE_TYPE RoleType { get; set; }
    }
}