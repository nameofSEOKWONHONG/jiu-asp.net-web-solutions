using System;
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

    protected virtual (bool isContinue, string message) OnPreExecute(ISessionContext sessionContext, TRequest request)
    {
        return new (true, string.Empty);
    }

    protected abstract TResult OnExecute(ISessionContext sessionContext, TRequest Request);

    protected virtual void OnPostExecute(ISessionContext sessionContext, TResult result)
    {
        
    }
    
    public virtual ResultBase<TResult> ExecuteCore()
    {
        var preExecutResult = OnPreExecute(_sessionContext, Request); 
        if (!preExecutResult.isContinue) return ResultBase<TResult>.Fail(preExecutResult.message);
        var result = OnExecute(_sessionContext, Request);
        OnPostExecute(_sessionContext, result);
        return ResultBase<TResult>.Success(result);
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
    private readonly IServiceBase<TRequest, TResult> _serviceBase;
    private readonly TransactionAttribute _transactionAttribute;
    
    public ServiceCore(IServiceBase<TRequest, TResult> serviceBase)
    {
        var transactionAttribute = _serviceBase.GetType().Assembly.GetCustomAttribute<TransactionAttribute>();
        if (transactionAttribute.xIsNotEmpty())
        {
            this._transactionAttribute = transactionAttribute;
        }
    }

    public virtual ResultBase<TResult> ExecuteCore()
    {
        ResultBase<TResult> result = null;
        
        if (this._transactionAttribute.xIsNotEmpty())
        {
            using var tran = new TransactionScope(this._transactionAttribute.TransactionScopeOption);
            result = _serviceBase.ExecuteCore();
            tran.Complete();
        }
        else
        {
            result = _serviceBase.ExecuteCore();
        }
        
        return result;
    }    
}

/// <summary>
///     service             >   repository
///     transactionscope
///                             DbContext
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValidator"></typeparam>
public sealed class ServiceCore<TRequest, TResult, TValidator> : ServiceCore<TRequest, TResult>
{
    private readonly IServiceBase<TRequest, TResult, TValidator> _serviceBase;
    private TransactionScopeOption _transactionScopeOption;
    public ServiceCore(IServiceBase<TRequest, TResult, TValidator> serviceBase) : base(serviceBase)
    {
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

/*
 *  Controller > Routed and Call ServiceCore
 *      - ServiceCore : Run ServiceBase
 *          -  ServiceBase : implement Service
 *              -   Repository : implement Repository > use dbContext
 */