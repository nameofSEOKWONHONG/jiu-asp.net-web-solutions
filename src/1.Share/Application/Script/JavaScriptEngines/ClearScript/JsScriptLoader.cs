using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.ClearScript;

public class JsScriptLoader : ScriptLoaderBase<IJsScripter>
{
    public JsScriptLoader(IOptions<ScriptLoaderConfig> options) : base(options)
    {
    }

    public IJsScripter Create(string fileName, string modulePath = null)
    {
        var fullFileName = Path.Combine(BasePath, fileName);
        if (Scriptors.TryGetValue(fileName, out IJsScripter scriptor))
        {
            return scriptor;
        }
        var newScriptor = new JsScripter(fullFileName, modulePath);
        if (Scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}