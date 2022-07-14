using Application.Infrastructure.Validation;
using Domain.Base;
using eXtensionSharp;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ActionBase : IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IHttpClientFactory _clientFactory;
    protected readonly IContextBase ContextBase;

    protected ActionBase(ILogger logger, IContextBase contextBase, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        ContextBase = contextBase;
        _clientFactory = clientFactory;
    }
    
    protected virtual (bool IsValid, DynamicDictionary<List<string>> Message) TryValidate<TEntity, TValidator>(TEntity entity)
        where TEntity : class
        where TValidator : AbstractValidator<TEntity>, new()
    {
        return ValidatorCore.TryValidate<TEntity, TValidator>(entity);
    }

    public virtual void Dispose()
    {
        Console.WriteLine("action dispose");
    }
}