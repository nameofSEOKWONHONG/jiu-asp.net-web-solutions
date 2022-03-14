using System;
using CSScripting;
using CSScriptLib;
using eXtensionSharp;

namespace Application.Script.SharpScript;

internal class SharpScripter : ISharpScripter
{
    private readonly ScriptItem _sharpScriptItem;
    public SharpScripter(string fileName)
    {
        _sharpScriptItem = new ScriptItem(fileName);
    }

    public TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : SharpScriptBase<TOptions, TRequest, TResult>, new()
    {
        var instance = new TInstance();
        instance.Options = options;
        instance.Request = request;
        instance.Execute();
        return instance.Result;
    }
    
    public TResult Execute<TOptions, TRequest, TResult>(TOptions options,
        TRequest request, string[] assemblies = null, Action<IEvaluator> assemblyTypeof = null)
    {
        var evaluator = CSScript.Evaluator.With(eval => eval.IsCachingEnabled = true);
        assemblies.xForEach(item => { evaluator.ReferenceAssemblyOf(item); });
        evaluator.ReferenceDomainAssemblies();
        var executor = evaluator.LoadCode<SharpScriptBase<TOptions, TRequest, TResult>>(_sharpScriptItem.Code);
        executor.Options = options;
        executor.Request = request;
        executor.Execute();
        return executor.Result;
    }
    
    public ISharpScriptBase AddExecutor<TOptions, TRequest>(TOptions options, TRequest request,
        string[] assemblies = null, Action<IEvaluator> assemblyTypeof = null)
    {
        var evaluator = CSScript.Evaluator.With(eval => eval.IsCachingEnabled = true);
        assemblies.xForEach(item => { evaluator.ReferenceAssemblyOf(item); });
        evaluator.ReferenceDomainAssemblies();
        var executor = evaluator.LoadCode<SharpScriptBase<TOptions, TRequest>>(_sharpScriptItem.Code);
        executor.Options = options;
        executor.Request = request;
        return executor;
    }
}