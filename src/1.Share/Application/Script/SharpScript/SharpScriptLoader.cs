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
        var fullPathName = Path.Combine(_basePath, fileName);
        if (_scriptors.TryGetValue(fileName, out ISharpScripter scriptor))
        {
            return scriptor;
        }
        
        var newScript = new SharpScripter(fullPathName);
        if (_scriptors.TryAdd(fileName, newScript))
        {
            return newScript;
        }

        return null;
    }
}