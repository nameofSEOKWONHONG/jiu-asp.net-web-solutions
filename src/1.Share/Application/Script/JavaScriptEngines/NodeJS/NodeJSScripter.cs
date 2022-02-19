using System;
using eXtensionSharp;
using Jering.Javascript.NodeJS;

namespace Application.Script.JavaScriptEngines.NodeJS;

/// <summary>
/// nodejs를 이용한 javascript 호출, nodejs를 사용하므로 nodejs에서 사용하는 모든 문법을 지원한다.
/// 먼저 nodejs 설치되어 있어야 하고 nodejs경로가 기본경로와 다를경우 config 설정에서 설정해 주어야 한다.
/// 아래 사이트 참조 (Javascript.NodeJS)
/// <see href="https://github.com/JeringTech/Javascript.NodeJS#debugging-javascript">
/// </summary>
internal class NodeJSScripter : INodeJSScripter
{
    private readonly ScriptItem _scriptItem;
    private readonly INodeJSService _nodeJsService;
    public NodeJSScripter(string fileName, INodeJSService nodeJsService)
    {
        _scriptItem = new ScriptItem(fileName);
        _nodeJsService = nodeJsService;
    }

    public void Execute<TResult>(Func<object[]> args, Action<TResult> executed)
    {
        object[] arguments = null;
        if (args.xIsNotEmpty()) arguments = args();
        var result = _nodeJsService.InvokeFromStringAsync<TResult>(_scriptItem.Code, args: arguments).GetAwaiter()
            .GetResult();
        executed(result);
    }
}