using System;

namespace Application.Script.JavaScriptEngines.NodeJS;

public interface INodeJSScripter
{
    void Execute<TResult>(Func<object[]> args, Action<TResult> executed);
}