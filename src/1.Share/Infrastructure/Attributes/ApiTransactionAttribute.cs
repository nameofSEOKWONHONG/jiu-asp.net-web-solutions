using System.Transactions;
using eXtensionSharp;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Attributes;

/// <summary>
/// Api TransactionScope
/// </summary>
public class ApiTransactionAttribute : Attribute, IAsyncActionFilter {
    //make sure filter marked as not reusable
    private readonly TransactionScopeOption _transactionScopeOption;

    public ApiTransactionAttribute(TransactionScopeOption transactionScopeOption) {
        _transactionScopeOption = transactionScopeOption;
    }

    public bool IsReusable => false;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        using (var transactionScope = new TransactionScope(_transactionScopeOption)) {
            var action = await next();
            if (action.Exception.xIsNull()) transactionScope.Complete();
        }
    }
}