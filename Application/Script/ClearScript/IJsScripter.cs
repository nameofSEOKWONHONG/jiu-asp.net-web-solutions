using System;
using Microsoft.ClearScript.V8;

namespace Application.Script.ClearScript;

public interface IJsScripter
{
    void Execute<TRequest>(TRequest request, Action<V8ScriptEngine> pre, Action<object> executed);
}