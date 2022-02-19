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
        var fullFileName = Path.Combine(_basePath, fileName);
        if (_scriptors.TryGetValue(fileName, out IJIntScripter scriptor))
        {
            return scriptor;
        }
        var newScriptor = new JIntScripter(fullFileName, modulePath);
        if (_scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}