using System.Collections.Concurrent;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Options;

namespace Application.Script.JavaScriptEngines.NodeJS;

public class NodeJSScriptLoader : IScriptReset
{
    public double Version { get; set; }

    private readonly ConcurrentDictionary<string, NodeJSScripter> _scriptors = new();
    private readonly INodeJSService _nodeJsService;
    public NodeJSScriptLoader(IOptions<ScriptLoaderConfig> options, INodeJSService nodeJsService)
    {
        this.Version = options.Value.Version;
        this._nodeJsService = nodeJsService;
    }
    
    public INodeJSScripter Create(string fileName)
    {
        var exists = this._scriptors.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var newCScriptor = new NodeJSScripter(fileName, _nodeJsService);
            if (_scriptors.TryAdd(fileName, newCScriptor))
            {
                return newCScriptor;    
            }
        }

        return exists.Value;
    }
    
    public bool Reset(string fileName = null)
    {
        if (this._scriptors.xIsEmpty()) return true;
        
        if(fileName.xIsEmpty()) this._scriptors.Clear();
        else
        {
            if (!_scriptors.TryRemove(fileName, out NodeJSScripter scriptor))
            {
                return false;
            }
        }
        return true;
    }
}