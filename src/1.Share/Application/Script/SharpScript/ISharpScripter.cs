using System;
using CSScriptLib;

namespace Application.Script.SharpScript;

public interface ISharpScripter
{
    TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : SharpScriptBase<TOptions, TRequest, TResult>, new();

    TResult Execute<TOptions, TRequest, TResult>(TOptions options,
        TRequest request, string[] assemblies = null, Action<IEvaluator> assemblyTypeof = null);

    ISharpScriptBase AddExecutor<TOptions, TRequest>(TOptions options, TRequest request, string[] assemblies = null,
        Action<IEvaluator> assemblyTypeof = null);
}