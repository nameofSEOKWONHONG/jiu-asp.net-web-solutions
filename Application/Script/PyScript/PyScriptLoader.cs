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
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fullPathName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new PyScripter(fullPathName, modulePath);
            if (_scriptors.TryAdd(fullPathName, newCScriptor))
            {
                return newCScriptor;    
            }
        }

        return exists.Value;
    }
}