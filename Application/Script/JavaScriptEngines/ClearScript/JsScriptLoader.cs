using System.Collections.Concurrent;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.ClearScript;

public class JsScriptLoader : IScriptReset
{
    public double Version { get; set; }
    private readonly ConcurrentDictionary<string, JsScripter> _scriptors = new ConcurrentDictionary<string, JsScripter>();
    
    public JsScriptLoader(IOptions<ScriptLoaderConfig> options)
    {
        this.Version = options.Value.Version;
    }

    public IJsScripter Create(string fileName, string modulePath = null)
    {
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new JsScripter(fileName, modulePath);
            if (_scriptors.TryAdd(fileName, newCScriptor))
            {
                return newCScriptor;    
            }
        }

        return exists.Value;
    }
    
    public bool Reset(string fileName = null)
    {
        if (this._scriptors.xIsEmpty()) return true;
        
        if(fileName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!_scriptors.TryRemove(fileName, out JsScripter scriptor))
            {
                return false;
            }
        }
        return true;
    }   
}