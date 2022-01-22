using System.Collections.Concurrent;
using System.Linq;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.PyScript;

public class PyScriptLoader : IScriptLoader
{
    public double Version { get; set; }
    private readonly ConcurrentDictionary<string, PyScriptor> _scriptors = new ConcurrentDictionary<string, PyScriptor>();
    public PyScriptLoader(IOptionsMonitor<ScriptLoaderConfig> options)
    {
        this.Version = options.CurrentValue.Version;
    }
    
    public IPyScriptor Create(string fileName, string[] modulePath = null)
    {
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new PyScriptor(fileName, modulePath);
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
            if (!_scriptors.TryRemove(fileName, out PyScriptor scriptor))
            {
                return false;
            }
        }
        return true;
    }   
}