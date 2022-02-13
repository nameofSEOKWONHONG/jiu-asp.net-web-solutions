using System;
using Microsoft.Scripting.Hosting;

namespace Application.Script.PyScript;

public interface IPyScripter
{
    void Execute(Action<ScriptScope> setAction, Action<ScriptScope> getAction);
} 