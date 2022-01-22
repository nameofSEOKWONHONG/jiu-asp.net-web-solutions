using System;
using Microsoft.Scripting.Hosting;

namespace Application.Script.PyScript;

public interface IPyScriptor
{
    void Execute(Action<ScriptScope> setAction, Action<ScriptScope> getAction);
} 