using System;
using eXtensionSharp;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Application.Script.PyScript;

internal class PyScripter : IPyScripter
{
    private readonly ScriptItem _scriptItem;
    private readonly string[] _modulePaths;
    
    public PyScripter(string fileName, string[] modulePaths = null)
    {
        var code = fileName.xFileReadAllText();
        _scriptItem = new ScriptItem(fileName, code, code.xToHash());
        _modulePaths = modulePaths;
    }

    public void Execute(Action<ScriptScope> setAction, Action<ScriptScope> getAction)
    {
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();
        if (_modulePaths.xIsNotEmpty())
        {
            var paths = engine.GetSearchPaths();
            _modulePaths.xForEach(item =>
            {
                paths.Add(item);
            });
            engine.SetSearchPaths(paths);
        }

        setAction(scope);
        engine.ExecuteFile(_scriptItem.FileName, scope);
        getAction(scope);
    }   
}