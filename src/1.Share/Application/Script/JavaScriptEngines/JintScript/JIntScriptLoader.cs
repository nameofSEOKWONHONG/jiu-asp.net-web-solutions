using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.JintScript;

public class JIntScriptLoader : ScriptLoaderBase<IJIntScripter>
{   
    public JIntScriptLoader(IOptions<ScriptLoaderConfig> options) : base(options)
    {
    }
    
    public IJIntScripter Create(string fileName, string modulePath = null)
    {
        var fullFileName = Path.Combine(BasePath, fileName);
        if (Scriptors.TryGetValue(fileName, out IJIntScripter scriptor))
        {
            return scriptor;
        }
        var newScriptor = new JIntScripter(fullFileName, modulePath);
        if (Scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}