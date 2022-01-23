using System;
using CSScriptLib;

namespace Application.Script.CsScript;

public interface ISharpScriptor
{
    TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : SharpScriptBase<TOptions, TRequest, TResult>, new();

    TResult Execute<TOptions, TRequest, TResult>(TOptions options,
        TRequest request, string[] assemblies = null, Action<IEvaluator> assemblyTypeof = null);
}