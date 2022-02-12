using System;
using System.Globalization;
using System.IO;
using Application.Script.TypeScript;
using CSScripting;
using eXtensionSharp;
using Jint;
using Jint.CommonJS;
using Jint.Native;
using Jint.Runtime.Debugger;
using JintEngine = Jint.Engine;

namespace Application.Script.JintScript;

internal class JIntScripter : IJIntScripter
{
    private readonly ScriptItem _scriptItem;
    private readonly CultureInfo _currentCulture;
    private readonly string _modulePath;
    
    public JIntScripter(string fileName, string modulePath = null, string cultureTxt = "ko-KR")
    {
        if (fileName.xContains(new[]{".ts"}))
        {
            var fromTsPath = Path.Combine(AppContext.BaseDirectory, fileName);
            //JINT는 import 구문을 지원하지 않으므로 Typescript의 모듈화를 적용할 수 없다.
            //대신 다중파일을 프리로드하여 진행할 수는 있으나 구문자체를 지원하지 않으므로 구조적 형태로 작성할 수 없다.
            //즉, ECMA SCRIPT규약 구문을 모두 지원해야만 사용 할 수 있다. 그 이전에는 부분적 유틸성 코드 작성만 가능한 것으로 함.
            //마지막이 Edge.js인데... 될 것 같은데...
            //Edge.js -> EdgeJs로 메인테너 변경되었고 아직 .net5/6 지원하지 않음.
            //언제 될지 모르겠고...
            //CsScript로 넘어간다. 자바스크립트는 포기함.
            
            //https://github.com/JeringTech/Javascript.NodeJS
            //위 프로젝트는 nodejs 프로세스를 .net host process에서 child process로 띄워서 사용할 수 있는 프로젝트이다.
            //따라서 nodejs 프로세스를 사용하므로 모든 기능이 사용가능한 것으로 보임.
            //마지막 도전은 위에 프로젝트로 진행함.
            //이전에 .net core 3에서 Microsoft.AspNetCore.NodeServices을 사용할 수 있었는데, .net 5에서 obsolete됨.
            //이후에 asp.net core에서 react 서버 랜더링을 지원하기 위해 Microsoft.AspNetCore.SpaServices.Extensions만 남은 것으로 보임.
            //위 프로젝트가 Microsoft.AspNetCore.NodeServices에서 포크되어 나온 것으로 확인됨.
            //nodejs가 설치되어 있어야 함.
            TypeScriptCompiler.Compile(fromTsPath);
            this._scriptItem = new ScriptItem(fileName);
            this._modulePath = modulePath;
            this._currentCulture = CultureInfo.GetCultureInfo(cultureTxt);            
        }
        else
        {
            this._scriptItem = new ScriptItem(fileName);
            this._modulePath = modulePath;
            this._currentCulture = CultureInfo.GetCultureInfo(cultureTxt);            
        }
    }
    
    public void Execute(Action<JintEngine> pre, Action<JintEngine> executed)
    {
        var engine = new JintEngine(cfg =>
        {
            cfg.Culture = this._currentCulture;
            cfg.DebugMode(true);
        });
        // List<Jint.Native.JsValue> jsValues = new List<Jint.Native.JsValue>();
        // _modulePaths.xForEach(path =>
        // {
        //     jsValues.Add(engine.CommonJS().RunMain(path));
        // });
        
        engine.DebugHandler.Step += (s, e) =>
        {
            //Console.WriteLine("{0}: Line {1}", e.CurrentStatement.ToString(), e.Location.Start.Line);
            return StepMode.Into;
        };
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