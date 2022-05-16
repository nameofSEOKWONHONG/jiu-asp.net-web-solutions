using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Application.Context;
using Application.Script.ClearScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Application.Script.SharpScript;
using Domain.Response;
using eXtensionSharp;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Jering.Javascript.NodeJS;
using Jint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Controllers;

public class ScriptEngineSampleController : ApiControllerBase<ScriptEngineSampleController>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SharpScriptLoader _sharpScriptLoader;
    private readonly JsScriptLoader _jsScriptLoader;
    private readonly PyScriptLoader _pyScriptLoader;
    private readonly JIntScriptLoader _jIntScriptLoader;
    private readonly NodeJSScriptLoader _nodeJsScriptLoader;
    private readonly INodeJSService _nodeJsService;
    
    public ScriptEngineSampleController(ApplicationDbContext dbContext,
        SharpScriptLoader sharpScriptLoader,
        JsScriptLoader jsScriptLoader,
        PyScriptLoader pyScriptLoader,
        JIntScriptLoader jIntScriptLoader,
        NodeJSScriptLoader nodeJsScriptLoader,
        INodeJSService nodeJsService)
    {
        _dbContext = dbContext;
        
        _sharpScriptLoader = sharpScriptLoader;
        _jsScriptLoader = jsScriptLoader;
        _pyScriptLoader = pyScriptLoader;
        _jIntScriptLoader = jIntScriptLoader;

        _nodeJsScriptLoader = nodeJsScriptLoader;
        _nodeJsService = nodeJsService;
    }

    [HttpGet("ScriptEngineSample")]
    public async Task<IActionResult> ScriptEngineSample()
    {
        #region [script execute sample]

        var csResult = _sharpScriptLoader.Create("HelloWorldScript.cs")
            .Execute<ApplicationDbContext, string, string>(_dbContext, "Run as CsScript",
                new[] { "System.Linq", "Application.Context", "Application.Script", "eXtensionSharp" });

        #region [scope sample]

        var scriptResult2 = _sharpScriptLoader.Create("HelloWorldScript.cs")
            .Execute<ApplicationDbContext, string, string>(_dbContext, "Run as CsScript",
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
        
        var result = string.Empty;
        if (result.xIsEmpty())
        {
            result = await Task.Factory.StartNew(() =>
            {
                var sb = new XStringBuilder();
                sb.AppendLine($"cs-script : {csResult}");
                sb.AppendLine($"js-script : {jsResult}");
                sb.AppendLine($"py-script : {pyResult}");
                sb.AppendLine($"jint-script : {jintResult}");
                sb.AppendLine($"ts-script : {tsResult}");
                sb.Release(out string str);
                return str;
            });
        }

        return Ok(result);
    }
    
    [HttpGet("NodeJSServiecSample")]
    public IActionResult NodeJSServiecSample()
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
        }, e => _logger.LogError(e, e.Message));
            
        return Ok(result);
    }
    public class NodeDataStructureSample
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
    }

    [HttpGet("PythonModulePathSample")]
    public IActionResult PythonModulePathSample()
    {
        var list = new List<string>();
        _pyScriptLoader.Create("version_sample.py").Execute(scope =>
        {
            ICollection<string> searchPaths = scope.Engine.GetSearchPaths();
            searchPaths.Add(@"D:\Microsoft Visual Studio\Shared\Python39_64\lib");
            searchPaths.Add(@"D:\Microsoft Visual Studio\Shared\Python39_64\libs");
            searchPaths.Add(@"D:\Microsoft Visual Studio\Shared\Python39_64\DLLs");
            scope.Engine.SetSearchPaths(searchPaths);
        }, scope =>
        {
            list.Add(scope.GetVariable<double>("n").ToString(CultureInfo.CurrentCulture));
            list.Add(scope.GetVariable<string>("text"));
        });

        return Ok(ResultBase<List<string>>.Success(list));
    } 
    
    [HttpGet("ScriptEnginesPerformanceDiff")]
    public IActionResult ScriptEnginesPerformanceDiff()
    {
        ResultBase<Dictionary<string, string>> result = new ResultBase<Dictionary<string, string>>();
        result.Data = new Dictionary<string, string>();

        var csResult = _sharpScriptLoader.Create("LoopSampleScript.cs")
            .Execute<bool, bool, string>(true, true, new[]
            {
                "System",
                "System.Diagnostics",
                "System.Threading.Tasks",
                "Application.Script.SharpScript",
                "Infrastructure.Persistence",
            });
        result.Data.Add("SharpScript", csResult.ToString());


        var jintResult = string.Empty;
        _jIntScriptLoader.Create("LoopSampleScript.js").Execute(
            engine =>
            {
                
            }, engine =>
            {
                jintResult = engine.GetValue("result").AsString();
            });
        
        result.Data.Add("JINTResult", jintResult);
        
        return Ok(result);
    }
}

