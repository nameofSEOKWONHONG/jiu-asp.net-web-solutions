using System.IO;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.PyScript;

public class PyScriptLoader : ScriptLoaderBase<IPyScripter>
{
    public PyScriptLoader(IOptions<ScriptLoaderConfig> options) : base(options)
    {
    }
    
    public IPyScripter Create(string fileName, string[] modulePath = null)
    {
        var fullPathName = Path.Combine(BasePath, fileName);
        if (Scriptors.TryGetValue(fileName, out IPyScripter scriptor))
        {
            return scriptor;
        }
        
        var newScriptor = new PyScripter(fullPathName, modulePath);
        if (Scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}