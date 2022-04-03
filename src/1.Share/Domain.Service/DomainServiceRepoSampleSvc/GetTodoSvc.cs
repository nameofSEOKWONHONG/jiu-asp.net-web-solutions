using System.Transactions;
using Application.Abstract;
using Domain.Entities;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace DomainServiceRepoSampleSvc;

public interface IGetTodoSvc : IServiceBase<int, TB_TODO>
{
    
}

[Transaction(TransactionScopeOption.Suppress)]
[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IGetTodoSvc))]
public class GetTodoSvc : ServiceBase<int, TB_TODO>, IGetTodoSvc
{
    private readonly ITodoRepository _todoRepository;
    public GetTodoSvc(ILogger logger, ISessionContext sessionContext, 
        ITodoRepository todoRepository) : base(logger, sessionContext)
    {
        _todoRepository = todoRepository;
    }

    protected override TB_TODO OnExecute(ISessionContext sessionContext, int Request)
    {
        return _todoRepository.Fetch(new TB_TODO() { ID = Request });
    }
}