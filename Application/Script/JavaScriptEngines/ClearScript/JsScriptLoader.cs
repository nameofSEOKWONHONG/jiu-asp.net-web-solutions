using System.Collections.Concurrent;
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
}