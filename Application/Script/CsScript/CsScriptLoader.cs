using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Script.CsScript;

public class CsScriptLoader
{
    private readonly CsScriptConfig _config;
    private CsScriptor _csScriptor;
    public CsScriptLoader(IOptions<CsScriptConfig> config)
    {
        _config = config.Value;
    }

    public CsScriptLoader Create(string fileName)
    {
        if(_csScriptor.xIsEmpty())
            _csScriptor = new CsScriptor(fileName);

        return this;
    }
    
    public TResult Execute<TInstance, TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null) where TInstance : CsScriptBase<TOptions, TRequest, TResult>, new()
    {
        if (_csScriptor.xIsNotEmpty()) 
            return _csScriptor.Execute<TInstance, TOptions, TRequest, TResult>(options, request, assemblies);

        return _csScriptor.Execute<TInstance, TOptions, TRequest, TResult>(options, request, assemblies);
    }

    public TResult Execute<TOptions, TRequest, TResult>(TOptions options, TRequest request,
        string[] assemblies = null)
    {
        if (_csScriptor.xIsNotEmpty()) 
            return _csScriptor.Execute<TOptions, TRequest, TResult>(options, request, assemblies);

        return _csScriptor.Execute<TOptions, TRequest, TResult>(options, request, assemblies);
    }
}