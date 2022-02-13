using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Application.Script.ClearScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Application.Script.SharpScript;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script;

public class ScriptLoaderBase<TScripter> : IScriptLoaderBase where TScripter : class
{
    public double Version { get; set; }
    protected readonly string _basePath = string.Empty;
    protected readonly ConcurrentDictionary<string, TScripter> _scriptors = new();

    private Dictionary<Type, Func<string>> _scriptTypeStates = new Dictionary<Type, Func<string>>()
    {
        { typeof(IPyScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\py") },
        { typeof(ISharpScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\cs") },
        { typeof(IJsScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\csscript") },
        { typeof(IJIntScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\jint") },
        { typeof(INodeJSScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\node") }
    };

    public ScriptLoaderBase(IOptions<ScriptLoaderConfig> options)
    {
        this.Version = options.Value.Version;
        this._basePath = options.Value.BasePath.xIfEmpty(() =>
        {
            return _scriptTypeStates[typeof(TScripter)]();
        });
    }
    
    public bool Reset(string fileName = null)
    {
        var ret = true;
        if (this._scriptors.xIsEmpty()) return true;
        
        fileName.xIfEmpty(
            () => this._scriptors.Clear(), 
            () => {
                var fullPathName = Path.Combine(_basePath, fileName);   
                if(fullPathName.xIsEmpty()) this._scriptors.Clear();
                else
                {
                    ret = _scriptors.TryRemove(fullPathName, out _);
                }
            });

        return ret;
    }
}