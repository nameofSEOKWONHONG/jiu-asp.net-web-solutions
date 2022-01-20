using System;
using System.Collections.Generic;
using Application.Script.CsScript;
using eXtensionSharp;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace Application.Script.ClearScript;

public class JScriptor
{
    private readonly CsScriptItem _mainJsItem;
    private readonly string _modulePath;
    public JScriptor(string mainJs, string modulePath = null)
    {
        var mainJsCode = mainJs.xFileReadAllText();
        _mainJsItem = new CsScriptItem(mainJs, mainJsCode, mainJsCode.xToHash());
        _modulePath = modulePath;
    }

    public void Execute<TRequest>(TRequest request, Action<V8ScriptEngine> action)
    {
        using var runtime = new V8Runtime();
        using var engine = runtime.CreateScriptEngine();
        if (_modulePath.xIsNotEmpty())
        {
            engine.DocumentSettings.SearchPath = _modulePath;
            engine.DocumentSettings.AccessFlags =
                DocumentAccessFlags.EnableFileLoading;
        }
        engine.AddHostType(typeof(Console));
        engine.Execute(
            new DocumentInfo() {Category = ModuleCategory.Standard},
            _mainJsItem.Code);
        var p = engine.Global.PropertyNames;
    }
}