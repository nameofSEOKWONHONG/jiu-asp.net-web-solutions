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
    protected readonly IClientSession ClientSession;

    protected ActionBase(ILogger logger, IClientSession clientSession, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        ClientSession = clientSession;
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