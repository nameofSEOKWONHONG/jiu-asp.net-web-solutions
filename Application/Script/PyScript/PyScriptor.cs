using System;
using eXtensionSharp;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Application.Script.PyScript;

internal class PyScriptor : IPyScriptor
{
    private readonly ScriptorItem _scriptorItem;
    private readonly string[] _modulePaths;
    
    public PyScriptor(string fileName, string[] modulePaths = null)
    {
        var code = fileName.xFileReadAllText();
        _scriptorItem = new ScriptorItem(fileName, code, code.xToHash());
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
        engine.ExecuteFile(_scriptorItem.FileName, scope);
        getAction(scope);
    }   
}