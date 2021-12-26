using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Context;
using Application.Interfaces.Todo;
using Domain.Entities;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace TodoApplication.Services
{
    public class TodoService : ITodoService
    {
        private readonly JIUDbContext _context;
        public TodoService(JIUDbContext context)
        {
            _context = context;
        }
        
        public async Task<Todo> GetTodoAsync(int id)
        {
            return await _context.Todos.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Todo>> GetAllTodoAsync(Guid userId)
        {
            return await _context.Todos.Where(m => m.WriteId == userId.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetTodoByDateAsync(Guid userId, DateTime selectedDate)
        {
            return await _context.Todos.Where(m => m.WriteId == userId.ToString() && 
                                                   (m.NotifyDate >= selectedDate.xToMin() && //2021-12-01 00:00:00
                                                    m.NotifyDate < selectedDate.xToMax())) // 2021-12-02 00:00:00
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetTodoByDateAsync(Guid userId, DateTime @from, DateTime @to)
        {
            return await _context.Todos.Where(m => m.WriteId == userId.ToString())
                .Where(m => m.WriteDt >= @from && m.WriteDt <= @to)
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetAllTodoByDateAsync(DateTime selectDate)
        {
            return await _context.Todos.Where(m => m.WriteDt < DateTime.Parse(selectDate.ToShortDateString()).AddDays(1))
                .ToListAsync();
        }

        public async Task<bool> RemoveTodoAsync(int id)
        {
            var exists = await GetTodoAsync(id);
            if (exists.xIsEmpty()) throw new KeyNotFoundException();
            var result = _context.Todos.Remove(exists);
            await _context.SaveChangesAsync();
            return result.State == EntityState.Deleted;
        }

        public async Task<Todo> InsertTodoAsync(Todo todo)
        {
            var result = await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo)
        {
            var exists = await GetTodoAsync(todo.Id);
            if (exists.xIsEmpty()) throw new KeyNotFoundException();
            exists.Contents = todo.Contents;
            exists.UpdateId = todo.UpdateId;
            exists.UpdateDt = todo.UpdateDt;
            var result = _context.Todos.Update(exists);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}