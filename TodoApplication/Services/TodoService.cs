using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Context;
using Application.Interfaces.Todo;
using Domain.Entities;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace TodoApplication.Services
{
    public class TodoService : ITodoService
    {
        private readonly JIUDbContext _context;
        public TodoService(JIUDbContext context)
        {
            _context = context;
        }
        
        public async Task<TB_TODO> GetTodoAsync(int id)
        {
            #region [sqlsugar sample]

            //var client = _context.GetSqlSugarClient();
            //return await client.Queryable<Todo>().Where(m => m.Id == id).FirstAsync();

            #endregion

            #region [sqlkata]

            var db = _context.GetSqlKataQueryFactory();
            return await db.Query("TB_TODO").Where("Id", id).FirstAsync<TB_TODO>();

            #endregion
            
            //return await _context.Todos.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<TB_TODO>> GetAllTodoAsync(Guid userId)
        {
            return await _context.Todos.Where(m => m.WRITE_ID == userId.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<TB_TODO>> GetTodoByDateAsync(Guid userId, DateTime selectedDate)
        {
            return await _context.Todos.Where(m => m.WRITE_ID == userId.ToString() && 
                                                   (m.NOTIFY_DT >= selectedDate.xToMin() && //2021-12-01 00:00:00
                                                    m.NOTIFY_DT < selectedDate.xToMax())) // 2021-12-02 00:00:00
                .ToListAsync();
        }

        public async Task<IEnumerable<TB_TODO>> GetTodoByDateAsync(Guid userId, DateTime @from, DateTime @to)
        {
            return await _context.Todos.Where(m => m.WRITE_ID == userId.ToString())
                .Where(m => m.WRITE_DT >= @from && m.WRITE_DT <= @to)
                .ToListAsync();
        }

        public async Task<IEnumerable<TB_TODO>> GetAllTodoByDateAsync(DateTime selectDate)
        {
            return await _context.Todos.Where(m => m.WRITE_DT < DateTime.Parse(selectDate.ToShortDateString()).AddDays(1))
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

        public async Task<TB_TODO> InsertTodoAsync(TB_TODO tbTodo)
        {
            var result = await _context.Todos.AddAsync(tbTodo);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<TB_TODO> UpdateTodoAsync(TB_TODO tbTodo)
        {
            var exists = await GetTodoAsync(tbTodo.ID);
            if (exists.xIsEmpty()) throw new KeyNotFoundException();
            exists.CONTENTS = tbTodo.CONTENTS;
            exists.UPDATE_ID = tbTodo.UPDATE_ID;
            exists.UPDATE_DT = tbTodo.UPDATE_DT;
            var result = _context.Todos.Update(exists);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}