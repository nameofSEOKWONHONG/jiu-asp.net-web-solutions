using System;
using System.Collections.Generic;
using Application.Script.CsScript;
using eXtensionSharp;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace Application.Script.ClearScript;

public interface IJsScriptor
{
    void Execute<TRequest>(TRequest request, Action<object> action);
}

internal class JsScriptor : IJsScriptor
{
    private readonly SharpScriptItem _mainJsItem;
    private readonly string _modulePath;
    public JsScriptor(string mainJs, string modulePath = null)
    {
        var mainJsCode = mainJs.xFileReadAllText();
        _mainJsItem = new SharpScriptItem(mainJs, mainJsCode, mainJsCode.xToHash());
        _modulePath = modulePath;
    }

    public void Execute<TRequest>(TRequest request, Action<object> action)
    {
        using (var engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDynamicModuleImports))
        {
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
            if (_modulePath.xIsNotEmpty())
            {
                engine.DocumentSettings.SearchPath = _modulePath;
                engine.DocumentSettings.AccessFlags =
                    DocumentAccessFlags.EnableFileLoading;
            }
            engine.AddHostType(typeof(Console));
            engine.DocumentSettings.ContextCallback = doc =>
            {
                if (doc.Name == "Logging.js")
                {
                    return new Dictionary<string, object> { { "Console", typeof(Console).ToHostType() } };
                }

                return null;
            };
            
            //마지막 결과만 반환한다.
            var result = engine.Evaluate(
                new DocumentInfo() {Category = ModuleCategory.Standard},
                _mainJsItem.Code);

            if (result.xIsNotEmpty())
            {
                action(result);
            }
        }
    }
}