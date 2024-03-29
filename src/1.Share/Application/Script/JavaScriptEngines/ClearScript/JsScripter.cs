﻿using System;
using System.Collections.Generic;
using eXtensionSharp;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace Application.Script.ClearScript;

internal class JsScripter : IJsScripter
{
    private readonly ScriptItem _mainJsItem;
    private readonly string _modulePath;
    public JsScripter(string fileName, string modulePath = null)
    {
        _mainJsItem = new ScriptItem(fileName);
        _modulePath = modulePath;
    } 

    public void Execute<TRequest>(TRequest request, Action<V8ScriptEngine> pre, Action<object> executed)
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
            // engine.AddHostType(typeof(Console));
            // engine.DocumentSettings.ContextCallback = doc =>
            // {
            //     if (doc.Name == "Logging.js")
            //     {
            //         return new Dictionary<string, object> { { "Console", typeof(Console).ToHostType() } };
            //     }
            //
            //     return null;
            // };
            pre(engine);
            
            //마지막 결과만 반환한다.
            var result = engine.Evaluate(
                new DocumentInfo() {Category = ModuleCategory.Standard},
                _mainJsItem.Code);

            if (result.xIsNotEmpty())
            {
                executed(result);
            }
        }
    }
}