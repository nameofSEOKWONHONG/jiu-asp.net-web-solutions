using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Domain.Response;
using eXtensionSharp;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace Application.Abstract;

public interface IServiceBase<TRequest, TResult>
{
    TRequest Request { get; set; }
    ResultBase<TResult> ExecuteCore();
}

public interface IServiceBase<TRequest, TResult, TValidator> : IServiceBase<TRequest, TResult>
{
    
}

public abstract class ServiceBase<TRequest, TResult> : IServiceBase<TRequest, TResult>
{
    protected readonly ILogger _logger;
    protected readonly ISessionContext _sessionContext;
    public TRequest Request { get; set; }
    public ServiceBase(ILogger logger, ISessionContext sessionContext)
    {
        _logger = logger;
        _sessionContext = sessionContext;
    }

    protected abstract (bool isContinue, string message) OnPreExecute(ISessionContext sessionContext, TRequest request);
    protected abstract ResultBase<TResult> OnExecute(ISessionContext sessionContext, TRequest Request);
    

    public virtual ResultBase<TResult> ExecuteCore()
    {
        var preExecutResult = OnPreExecute(_sessionContext, Request); 
        if (!preExecutResult.isContinue) return ResultBase<TResult>.Fail(preExecutResult.message);
        return OnExecute(_sessionContext, Request);
    }
}

public abstract class ServiceBase<TRequest, TResult, TValidator> : ServiceBase<TRequest, TResult>, IServiceBase<TRequest, TResult, TValidator>
    where TValidator : AbstractValidator<TRequest>, new()
{
    public ServiceBase(ILogger logger, ISessionContext sessionContext) : base(logger, sessionContext) 
    {
    }

    public override ResultBase<TResult> ExecuteCore()
    {
        var validator = new TValidator();
        var result = validator.Validate(this.Request);
        if(!result.IsValid) return ResultBase<TResult>.Fail(string.Join(", ", result.Errors.Select(m => m.ErrorMessage)));
        return base.ExecuteCore();
    }
}

public class ServiceCore<TRequest, TResult>
{
    private readonly HashSet<IServiceBase<TRequest, TResult>> _serviceBases;
    private readonly TransactionScopeOption[] _transactionScopeOptions;
    
    public ServiceCore(IServiceBase<TRequest, TResult> serviceBase)
    {
        _serviceBases = new HashSet<IServiceBase<TRequest, TResult>>();
        _serviceBases.Add(serviceBase);
        var customAttributesServiceBases = _serviceBases.Where(m => m.GetType().Assembly.GetCustomAttribute<TransactionAttribute>() != null);
        if (customAttributesServiceBases.xIsNotEmpty())
        {
            var trans = new List<TransactionScopeOption>();
            customAttributesServiceBases.xForEach(item =>
            {
                var customAttribute = item.GetType().GetCustomAttribute<TransactionAttribute>();
                trans.Add(customAttribute.TransactionScopeOption);
            });
        }
    }

    public ServiceCore(IServiceBase<TRequest, TResult>[] serviceBases)
    {
        
    }

    public ResultBase<TResult> ExecuteCore()
    {
        var trans = new List<TransactionScope>();
        _serviceBases.xForEach((item, i) =>
        {
            var tran = new TransactionScope(_transactionScopeOptions[i]);
            trans.Add(tran);
            item.ExecuteCore();
        });
        return null;
    }    
}

public sealed class ServiceCore<TRequest, TResult, TValidator>
{
    private readonly IServiceBase<TRequest, TResult, TValidator> _serviceBase;
    private TransactionScopeOption _transactionScopeOption;
    public ServiceCore(IServiceBase<TRequest, TResult, TValidator> serviceBase)
    {
        _serviceBase = serviceBase;
    }
    
    public ResultBase<TResult> ExecuteCore()
    {
        ResultBase<TResult> result = null;

        using var tran = new TransactionScope(_transactionScopeOption);
        result = _serviceBase.ExecuteCore();
        tran.Complete();

        return result;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class TransactionAttribute : Attribute
{
    public readonly TransactionScopeOption TransactionScopeOption;
    public TransactionAttribute(TransactionScopeOption transactionScopeOption)
    {
        TransactionScopeOption = transactionScopeOption;
    }
}