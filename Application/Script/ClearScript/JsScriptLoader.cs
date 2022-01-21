using System;
using System.Collections.Concurrent;
using System.Linq;
using eXtensionSharp;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Application.Script.ClearScript;

public class JsScriptLoader
{
    private readonly ConcurrentDictionary<string, JsScriptor> _csriptors = new ConcurrentDictionary<string, JsScriptor>();
    
    public JsScriptLoader()
    {
        
    }

    public IJsScriptor Create(string fileName, string modulePath = null)
    {
        var exists = this._csriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new JsScriptor(fileName, modulePath);
            if (_csriptors.TryAdd(fileName, newCScriptor))
            {
                return newCScriptor;    
            }
        }

        return exists.Value;
    }
    
    public bool Reset(string fileName = null)
    {
        if(fileName.xIsEmpty()) this._csriptors.Clear();
        else
        {
            if (!_csriptors.TryRemove(fileName, out JsScriptor scriptor))
            {
                return false;
            }
        }
        return true;
    }    
}