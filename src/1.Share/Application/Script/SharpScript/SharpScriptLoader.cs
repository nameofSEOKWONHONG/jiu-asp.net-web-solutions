using System;
using System.Collections.Concurrent;
using System.IO;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.SharpScript;


public class SharpScriptLoader : ScriptLoaderBase<ISharpScripter>
{
    public SharpScriptLoader(IOptions<ScriptLoaderConfig> options) : base(options)
    {
    }

    public ISharpScripter Create(string fileName)
    {
        var fullPathName = Path.Combine(BasePath, fileName);
        if (Scriptors.TryGetValue(fileName, out ISharpScripter scriptor))
        {
            return scriptor;
        }
        
        var newScript = new SharpScripter(fullPathName);
        if (Scriptors.TryAdd(fileName, newScript))
        {
            return newScript;
        }

        return null;
    }
}