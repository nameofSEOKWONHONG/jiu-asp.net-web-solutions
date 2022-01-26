using System;
using Jint.Native;
using JintEngine = Jint.Engine;

namespace Application.Script.JintScript;

public interface IJIntScripter
{
    void Execute(Action<JintEngine> pre, Action<JintEngine> executed);
    void Execute(Action<JintEngine> pre, Action<JsValue> executed);
}