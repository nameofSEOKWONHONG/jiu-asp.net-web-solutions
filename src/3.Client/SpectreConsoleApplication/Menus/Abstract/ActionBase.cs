using Application.Infrastructure.Validation;
using eXtensionSharp;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ActionBase
{
    protected readonly ILogger _logger;
    protected readonly IHttpClientFactory _clientFactory;

    protected ActionBase(ILogger logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }
    
    protected virtual (bool IsValid, DynamicDictionary<List<string>> Message) TryValidate<TEntity, TValidator>(TEntity entity)
        where TEntity : class
        where TValidator : AbstractValidator<TEntity>, new()
    {
        return ValidatorCore.TryValidate<TEntity, TValidator>(entity);
    }
}