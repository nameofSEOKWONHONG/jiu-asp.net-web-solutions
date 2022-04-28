using System.Linq.Expressions;
using System.Transactions;
using Application.Base;
using Application.Context;
using Domain.Dtos;
using Domain.Entities;
using eXtensionSharp;
using InjectionExtension;

namespace DomainServiceRepoSampleSvc;

public interface ITodoRepository : IRepositoryBase<TB_TODO> {}

[Transaction(TransactionScopeOption.Suppress)]
[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(ITodoRepository))]
public class TodoRepository : RepositoryBase<ApplicationDbContext, TB_TODO>, ITodoRepository 
{
    private readonly List<PersonDto> _items;
    public TodoRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override TB_TODO Fetch(TB_TODO item)
    {
        return _dbContext.Todos.FirstOrDefault(m => m.ID == item.ID);
    }

    public override IEnumerable<TB_TODO> Query(TB_TODO request, int currentPage = 1, int pageSize = 50)
    {
        return _dbContext.Todos.Skip(currentPage - 1 * pageSize).Take(pageSize);
    }
}
