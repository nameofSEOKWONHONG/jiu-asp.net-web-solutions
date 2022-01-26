using System.Collections.Concurrent;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.CsScript;

public class SharpScriptLoader : IScriptReset
{
    public double Version { get; set; }
    private readonly ConcurrentDictionary<string, SharpScripter> _scriptors = new ConcurrentDictionary<string, SharpScripter>();
    
    public SharpScriptLoader(IOptionsMonitor<ScriptLoaderConfig> options)
    {
        this.Version = options.CurrentValue.Version;
    }

    public ISharpScripter Create(string fileName)
    {
        if (_scriptors.TryGetValue(fileName, out SharpScripter scriptor))
        {
            return scriptor;
        }
        
        var newScript = new SharpScripter(fileName);
        if (_scriptors.TryAdd(fileName, newScript))
        {
            return newScript;
        }

        return null;
    }

    public bool Reset(string fileName = null)
    {
        if (this._scriptors.xIsEmpty()) return true;
        
        if(fileName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!this._scriptors.TryRemove(fileName, out SharpScripter csScriptor))
            {
                return false;
            }
        }

        return true;
    }
}