using System.Collections.Concurrent;
using eXtensionSharp;

namespace Application.Script.CsScript;

public class SharpScriptLoader
{
    private readonly ConcurrentDictionary<string, SharpScriptor> _scriptors = new ConcurrentDictionary<string, SharpScriptor>();
    
    public SharpScriptLoader()
    {
        
    }

    public ISharpScriptor Create(string fileName)
    {
        if (_scriptors.TryGetValue(fileName, out SharpScriptor scriptor))
        {
            return scriptor;
        }
        
        var newScript = new SharpScriptor(fileName);
        if (_scriptors.TryAdd(fileName, newScript))
        {
            return newScript;
        }

        return null;
    }

    public bool Reset(string fileName = null)
    {
        if(fileName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!this._scriptors.TryRemove(fileName, out SharpScriptor csScriptor))
            {
                return false;
            }
        }

        return true;
    }
}