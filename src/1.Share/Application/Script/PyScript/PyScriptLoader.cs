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
        var fullPathName = Path.Combine(_basePath, fileName);
        if (_scriptors.TryGetValue(fileName, out IPyScripter scriptor))
        {
            return scriptor;
        }
        
        var newScriptor = new PyScripter(fullPathName, modulePath);
        if (_scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}