﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Application.Script.ClearScript;
using Application.Script.CsScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script;

public class ScriptLoaderBase<TScripter> : IScriptLoaderBase where TScripter : class
{
    public double Version { get; set; }
    protected readonly string _basePath = string.Empty;
    protected readonly ConcurrentDictionary<string, TScripter> _scriptors = new ConcurrentDictionary<string, TScripter>();

    private Dictionary<Type, Func<string>> _scriptTypeStates = new Dictionary<Type, Func<string>>()
    {
        { typeof(PyScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\py") },
        { typeof(SharpScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\cs") },
        { typeof(JsScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\csscript") },
        { typeof(JIntScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\js\\jint") },
        { typeof(NodeJSScripter), () => Path.Combine(AppContext.BaseDirectory, "ScriptFiles\\node") }
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
        if (this._scriptors.xIsEmpty()) return true;
        
        var fullPathName = Path.Combine(_basePath, fileName);   
        if(fullPathName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!_scriptors.TryRemove(fullPathName, out TScripter scriptor))
            {
                return false;
            }
        }
        return true;
    }
}