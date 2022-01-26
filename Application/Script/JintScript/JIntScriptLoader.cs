using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Application.Script.ClearScript;
using CSScripting;
using eXtensionSharp;
using Jint;
using Jint.CommonJS;
using Jint.Native;
using Microsoft.ClearScript.V8;
using JintEngine = Jint.Engine;

namespace Application.Script.JintScript;

public interface IJIntScripter
{
    void Execute<TRequest>(TRequest request, Action<JintEngine> pre, Action<JintEngine> executed);
    void ExecuteModule<TRequest>(TRequest request, Action<JintEngine> pre, Action<JsValue> executed);
}

public class JIntScripter : IJIntScripter
{
    private readonly ScriptItem _scriptItem;
    private readonly CultureInfo _currentCulture;
    private readonly string _modulePath; 
    public JIntScripter(string mainJs, string modulePath = null, string cultureTxt = "ko-KR")
    {
        var code = mainJs.xFileReadAllText();
        this._scriptItem = new ScriptItem(mainJs, code, code.xToHash());
        this._modulePath = modulePath;
        this._currentCulture = CultureInfo.GetCultureInfo(cultureTxt);
    }
    
    public void Execute<TRequest>(TRequest request, Action<JintEngine> pre, Action<JintEngine> executed)
    {
        var engine = new JintEngine(cfg => cfg.Culture = this._currentCulture);
        // List<Jint.Native.JsValue> jsValues = new List<Jint.Native.JsValue>();
        // _modulePaths.xForEach(path =>
        // {
        //     jsValues.Add(engine.CommonJS().RunMain(path));
        // });
        pre(engine);
        engine.Execute(this._scriptItem.Code);
        executed(engine);
    }

    public void ExecuteModule<TRequest>(TRequest request, Action<JintEngine> pre, Action<JsValue> executed)
    {
        Directory.SetCurrentDirectory(Path.GetTempPath());
        var engine = new JintEngine(cfg => cfg.Culture = this._currentCulture);
        pre(engine);
        var code = _modulePath.xFileReadAllText();
        var fileName = _modulePath.GetFileName();
        File.WriteAllText(fileName, code);
        var jsValue = engine.CommonJS().RunMain(Path.Combine("./", fileName.GetFileNameWithoutExtension()));
        executed(jsValue);
    }
}

public interface IJIntScriptLoader
{
    
}
public class JIntScriptLoader
{
    public double Version { get; set; }
    private readonly ConcurrentDictionary<string, JsScripter> _scriptors = new ConcurrentDictionary<string, JsScripter>();
    
    public JIntScriptLoader()
    {
        
    }
    public void Execute<TRequest>(TRequest request, Action<V8ScriptEngine> pre, Action<object> executed)
    {
        throw new NotImplementedException();
    }
}