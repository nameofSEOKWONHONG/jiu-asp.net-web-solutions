using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Todo
{
    public interface ITodoService
    {
        Task<Domain.Entities.Todo> GetTodoAsync(int id);
        Task<IEnumerable<Domain.Entities.Todo>> GetAllTodoAsync(Guid userId);
        Task<IEnumerable<Domain.Entities.Todo>> GetTodoByDateAsync(Guid userId, DateTime selectedDate);
        Task<IEnumerable<Domain.Entities.Todo>> GetTodoByDateAsync(Guid userId, DateTime @from, DateTime @to);
        Task<Domain.Entities.Todo> InsertTodoAsync(Domain.Entities.Todo todo);
        Task<Domain.Entities.Todo> UpdateTodoAsync(Domain.Entities.Todo todo);
        Task<bool> RemoveTodoAsync(int id);
    }
}