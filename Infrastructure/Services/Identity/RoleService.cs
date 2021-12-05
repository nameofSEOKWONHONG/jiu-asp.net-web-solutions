using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using eXtensionSharp;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{    
    public class RoleService : IRoleService
    {
        private readonly JIUDbContext _dbContext;
        public RoleService(JIUDbContext context)
        {
            _dbContext = context;
        }
        public async Task<Role> GetRole(int roleId)
        {
            return await _dbContext.Roles.FirstAsync(m => m.Id == roleId);
        }

        public async Task<IEnumerable<Role>> GetRole()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role> SaveRole(Role role)
        {
            Role result = null;
            var exists = await _dbContext.Roles.FirstAsync(m => m.RoleType == role.RoleType);
            if (exists.xIsEmpty())
            {
                //save
                var entityEntry = _dbContext.Roles.Add(role);
                result = entityEntry.Entity;
            }
            else
            {
                //update   
                // exists.RoleClaim = role.RoleClaim;
                result = exists;
            }

            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<Role> RemoveRole(int id)
        {
            var exists = await _dbContext.Roles.FirstAsync(m => m.Id == id);
            if (exists.xIsEmpty()) throw new Exception("Not found role");

            var resultEntity = _dbContext.Roles.Remove(exists);
            await _dbContext.SaveChangesAsync();
            return resultEntity.Entity;
        }
    }
}