using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoService.Services
{
    public interface ITodoService
    {
        Task<Domain.Entities.TB_TODO> GetTodoAsync(int id);
        Task<IEnumerable<Domain.Entities.TB_TODO>> GetAllTodoAsync(Guid userId);
        Task<IEnumerable<Domain.Entities.TB_TODO>> GetTodoByDateAsync(Guid userId, DateTime selectedDate);
        Task<IEnumerable<Domain.Entities.TB_TODO>> GetTodoByDateAsync(Guid userId, DateTime @from, DateTime @to);
        Task<IEnumerable<Domain.Entities.TB_TODO>> GetAllTodoByDateAsync(DateTime selectDate);
        Task<Domain.Entities.TB_TODO> InsertTodoAsync(Domain.Entities.TB_TODO tbTodo);
        Task<Domain.Entities.TB_TODO> UpdateTodoAsync(Domain.Entities.TB_TODO tbTodo);
        Task<bool> RemoveTodoAsync(int id);
    }
}