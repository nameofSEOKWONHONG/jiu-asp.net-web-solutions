using System;
using System.Reflection;
using System.Transactions;
using Domain.Response;
using eXtensionSharp;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace Application.Base;

public interface IServiceBase
{
    void ExecuteCore();
}

public interface IServiceBase<TRequest, TResult> : IServiceBase
{
    TRequest Request { get; set; }
    TResult Result { get; }
}

public interface IServiceBase<TRequest, TResult, TValidator> : IServiceBase<TRequest, TResult>
{
    
}

public abstract class ServiceBase<TRequest, TResult> : IServiceBase<TRequest, TResult>
{
    protected readonly ILogger _logger;
    protected readonly ISessionContext _sessionContext;
    public TRequest Request { get; set; }
    public TResult Result { get; private set; }
    public ServiceBase(ILogger logger, ISessionContext sessionContext)
    {
        _logger = logger;
        _sessionContext = sessionContext;
    }

    protected virtual (bool isContinue, string message) OnPreExecute(ISessionContext sessionContext, TRequest request)
    {
        return new (true, string.Empty);
    }

    protected abstract TResult OnExecute(ISessionContext sessionContext, TRequest request);

    protected virtual void OnPostExecute(ISessionContext sessionContext, TResult result)
    {
        
    }
    
    public virtual void ExecuteCore()
    {
        var preExecutResult = OnPreExecute(_sessionContext, Request);
        if (!preExecutResult.isContinue) return;
        this.Result = OnExecute(_sessionContext, Request);
        OnPostExecute(_sessionContext, this.Result);
    }
}

public abstract class ServiceBase<TRequest, TResult, TValidator> : ServiceBase<TRequest, TResult>, IServiceBase<TRequest, TResult, TValidator>
    where TValidator : IValidator<TRequest>, new()
{
    public ServiceBase(ILogger logger, ISessionContext sessionContext) : base(logger, sessionContext) 
    {
    }

    public override void ExecuteCore()
    {
        var validator = new TValidator();
        var result = validator.Validate(this.Request);
        if (!result.IsValid) return;
        base.ExecuteCore();
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
            _serviceBase.ExecuteCore();
            result = ResultBase<TResult>.Success(_serviceBase.Result);
            tran.Complete();
        }
        else
        {
            result = ResultBase<TResult>.Success(_serviceBase.Result);
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

/// <summary>
/// TransactionScope Attribute
/// <caution>dotnet core and higher version not support multi database transaction.</caution> 
/// </summary>
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