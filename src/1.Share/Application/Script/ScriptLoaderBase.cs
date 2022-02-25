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

public class ScriptLoaderState
{
    public static readonly Dictionary<Type, Func<string>> ScriptTypeStates = new()
    {
        { typeof(IPyScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\py") },
        { typeof(ISharpScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\cs") },
        { typeof(IJsScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\csscript") },
        { typeof(IJIntScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\jint") },
        { typeof(INodeJSScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\node") }
    };
}

public class ScriptLoaderBase<TScripter> : IScriptLoaderBase where TScripter : class
{
    public double Version { get; set; }
    protected readonly string BasePath = string.Empty;
    protected readonly ConcurrentDictionary<string, TScripter> Scriptors = new();

    protected ScriptLoaderBase(IOptions<ScriptLoaderConfig> options)
    {
        this.Version = options.Value.Version;
        this.BasePath = options.Value.BasePath.xIfEmpty(() => ScriptLoaderState.ScriptTypeStates[typeof(TScripter)]());
    }
    
    public bool Reset(string fileName = null)
    {
        var ret = true;
        if (this.Scriptors.xIsEmpty()) return true;
        
        fileName.xIfEmpty(
            () => this.Scriptors.Clear(), 
            () => {
                var fullPathName = Path.Combine(BasePath, fileName);   
                if(fullPathName.xIsEmpty()) this.Scriptors.Clear();
                else
                {
                    ret = Scriptors.TryRemove(fullPathName, out _);
                }
            });

        return ret;
    }
}