using System;
using CSScripting;
using CSScriptLib;
using eXtensionSharp;

namespace Application.Script.CsScript;

internal class SharpScripter : ISharpScripter
{
    private readonly ScriptItem _sharpScriptItem;
    public SharpScripter(string fileName)
    {
        var code = fileName.xFileReadAllText();
        _sharpScriptItem = new ScriptItem(fileName, code, code.xToHash());
    }

    public TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : SharpScriptBase<TOptions, TRequest, TResult>, new()
    {
        var instance = new TInstance();
        instance.Options = options;
        instance.Request = request;
        if (!instance.Validate(out string message)) throw new Exception(message);
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

        if (!executor.Validate(out string message)) throw new Exception(message);

        executor.Execute();
        return executor.Result;
    }
}