using System.Collections.Concurrent;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Options;

namespace Application.Script.JavaScriptEngines.NodeJS;

/// <summary>
/// node 모듈화하여 실행하기 위해서는 ProjectPath를 설정해야 한다.
/// 시작 부분이 파일이든 String 코드이던 ProjectPath가 설정되어 있다면 모듈화 하여 사용할 수 있다.
/// </summary>
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