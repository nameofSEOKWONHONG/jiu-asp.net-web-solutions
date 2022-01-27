using System.Collections.Concurrent;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.JintScript;

public class JIntScriptLoader : IScriptReset
{
    public double Version { get; set; }

    private readonly ConcurrentDictionary<string, JIntScripter> _scriptors = new();
    
    public JIntScriptLoader(IOptions<ScriptLoaderConfig> options)
    {
        this.Version = options.Value.Version;
    }
    
    public IJIntScripter Create(string fileName, string modulePath = null)
    {
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new JIntScripter(fileName, modulePath);
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
            if (!_scriptors.TryRemove(fileName, out JIntScripter scriptor))
            {
                return false;
            }
        }
        return true;
    }
}