using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eXtensionSharp;
using FluentValidation;

namespace Application.Infrastructure.Validation;

public class ValidatorCore
{
    protected ValidatorCore()
    {
        
    }
    
    public static (bool IsValid, DynamicDictionary<List<string>> Message) TryValidate<TEntity, TValidator>(TEntity entity)
        where TEntity : class
        where TValidator : AbstractValidator<TEntity>, new()
    {
        var validator = new TValidator();
        var validateResult = validator.Validate(entity);
        var messageMap = new DynamicDictionary<List<string>>();
        if (validateResult.IsValid.xIsFalse())
        {
            validateResult.Errors.xForEach(m =>
            {
                var exists = messageMap.FirstOrDefault(item => item.Key == m.PropertyName);
                if (exists.xIsNotEmpty())
                {
                    exists.Value.Add(m.ErrorMessage);
                }
                else
                {
                    messageMap.Add(m.PropertyName, new List<string>() {m.ErrorMessage});    
                }
            });
        }

        return new ValueTuple<bool, DynamicDictionary<List<string>>>(validateResult.IsValid, messageMap);
    } 
    
    public static async Task<(bool IsValid, DynamicDictionary<List<string>> Message)> TryValidateAsync<TEntity, TValidator>(TEntity entity) 
        where TEntity : class
        where TValidator : AbstractValidator<TEntity>, new()
    {
        var validator = new TValidator();
        var validateResult = await validator.ValidateAsync(entity);
        var messageMap = new DynamicDictionary<List<string>>();
        if (validateResult.IsValid.xIsFalse())
        {
            validateResult.Errors.xForEach(m =>
            {
                var exists = messageMap.FirstOrDefault(item => item.Key == m.PropertyName);
                if (exists.xIsNotEmpty())
                {
                    exists.Value.Add(m.ErrorMessage);
                }
                else
                {
                    messageMap.Add(m.PropertyName, new List<string>() {m.ErrorMessage});    
                }
            });
        }

        return new ValueTuple<bool, DynamicDictionary<List<string>>>(validateResult.IsValid, messageMap);
    }
}