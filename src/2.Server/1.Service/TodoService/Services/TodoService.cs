﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Context;
using Domain.Entities;
using eXtensionSharp;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TodoService.Services
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;
        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<TB_TODO> GetTodoAsync(int id)
        {
            #region [sqlsugar sample]

            // var context = new SqlSugarDbContext(this._context.Database.GetConnectionString(), DbType.SqlServer);
            // var client = context.GetSqlSugarClient();
            //return await client.Queryable<Todo>().Where(m => m.ID == id).FirstAsync();

            #endregion

            #region [sqlkata]

            // var context = new SqlKataDbContext(this._context.Database.GetConnectionString(), ENUM_DATABASE_TYPE.MSSQL);
            // var db = context.GetSqlKataQueryFactory();
            // var tran = db.connection.BeginTransaction();
            // try
            // {
            //     var result = await db.queryFactory.Query("TB_TODO").Where("ID", id).FirstAsync<TB_TODO>();
            //     tran.Commit();
            // }
            // finally
            // {
            //     tran.Rollback();
            // }

            #endregion

            #region [ef core]

            return await _context.Todos.FirstOrDefaultAsync(m => m.ID == id);

            #endregion
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