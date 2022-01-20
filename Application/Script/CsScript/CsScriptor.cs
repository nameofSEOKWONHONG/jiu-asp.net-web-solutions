using System;
using CSScripting;
using CSScriptLib;
using eXtensionSharp;

namespace Application.Script.CsScript;

internal class CsScriptor
{
    private readonly CsScriptItem _csCsScriptItem;
    public CsScriptor(string fileName)
    {
        var code = fileName.xFileReadAllText();
        _csCsScriptItem = new CsScriptItem(fileName, code, code.xToHash());
    }

    public TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : CsScriptBase<TOptions, TRequest, TResult>, new()
    {
        var instance = new TInstance();
        instance.Options = options;
        instance.Request = request;
        if (!instance.Validate(out string message)) throw new Exception(message);
        instance.Execute();
        return instance.Result;
    }
    
    public TResult Execute<TOptions, TRequest, TResult>(TOptions options,
        TRequest request, string[] assemblies = null)
    {
        var evaluator = CSScript.Evaluator.With(eval => eval.IsCachingEnabled = true);
        assemblies.xForEach(item => { evaluator.ReferenceAssemblyOf(item); });
        evaluator.ReferenceDomainAssemblies();
        var executor = evaluator.LoadCode<CsScriptBase<TOptions, TRequest, TResult>>(_csCsScriptItem.Code);
        executor.Options = options;
        executor.Request = request;

        if (!executor.Validate(out string message)) throw new Exception(message);

        executor.Execute();
        return executor.Result;

        return default;
    }
}