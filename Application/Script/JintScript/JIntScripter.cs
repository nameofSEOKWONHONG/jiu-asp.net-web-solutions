using System;
using System.Globalization;
using System.IO;
using Application.Script.TypeScript;
using CSScripting;
using eXtensionSharp;
using Jint.CommonJS;
using Jint.Native;
using JintEngine = Jint.Engine;

namespace Application.Script.JintScript;

public class JIntScripter : IJIntScripter
{
    private readonly ScriptItem _scriptItem;
    private readonly CultureInfo _currentCulture;
    private readonly string _modulePath; 
    public JIntScripter(string mainJs, string modulePath = null, string cultureTxt = "ko-KR")
    {
        if (mainJs.xContains(new[]{".ts"}))
        {
            TypeScriptCompiler.Compile(mainJs);
            var fileName = mainJs.GetFileNameWithoutExtension();
            var jsCode = $"{mainJs.Replace(".ts", ".js")}".xFileReadAllText();
            this._scriptItem = new ScriptItem(mainJs, jsCode, jsCode.xToHash());
            this._modulePath = modulePath;
            this._currentCulture = CultureInfo.GetCultureInfo(cultureTxt);            
        }
        else
        {
            var code = mainJs.xFileReadAllText();
            this._scriptItem = new ScriptItem(mainJs, code, code.xToHash());
            this._modulePath = modulePath;
            this._currentCulture = CultureInfo.GetCultureInfo(cultureTxt);            
        }
    }
    
    public void Execute(Action<JintEngine> pre, Action<JintEngine> executed)
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

    public void Execute(Action<JintEngine> pre, Action<JsValue> executed)
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