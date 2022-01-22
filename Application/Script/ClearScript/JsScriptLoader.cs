﻿using System.Collections.Concurrent;
using System.Linq;
using eXtensionSharp;

namespace Application.Script.ClearScript;

public class JsScriptLoader
{
    private readonly ConcurrentDictionary<string, JsScriptor> _scriptors = new ConcurrentDictionary<string, JsScriptor>();
    
    public JsScriptLoader()
    {
        
    }

    public IJsScriptor Create(string fileName, string modulePath = null)
    {
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new JsScriptor(fileName, modulePath);
            if (_scriptors.TryAdd(fileName, newCScriptor))
            {
                return newCScriptor;    
            }
        }

        return exists.Value;
    }
    
    public bool Reset(string fileName = null)
    {
        if(fileName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!_scriptors.TryRemove(fileName, out JsScriptor scriptor))
            {
                return false;
            }
        }
        return true;
    }    
}