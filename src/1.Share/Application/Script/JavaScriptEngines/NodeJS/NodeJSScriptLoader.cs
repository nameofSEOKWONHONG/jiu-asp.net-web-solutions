using System.Collections.Concurrent;
using System.IO;
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
public class NodeJSScriptLoader : ScriptLoaderBase<INodeJSScripter>
{
    private readonly INodeJSService _nodeJsService;
    public NodeJSScriptLoader(INodeJSService nodeJsService, IOptions<ScriptLoaderConfig> options) : base(options)
    {
        this._nodeJsService = nodeJsService;
    }
    
    public INodeJSScripter Create(string fileName)
    {
        var fullFileName = Path.Combine(BasePath, fileName);
        if (Scriptors.TryGetValue(fileName, out INodeJSScripter scriptor))
        {
            return scriptor;
        }
        var newScriptor = new NodeJSScripter(fullFileName, _nodeJsService);
        if (Scriptors.TryAdd(fileName, newScriptor))
        {
            return newScriptor;    
        }

        return null;
    }
}