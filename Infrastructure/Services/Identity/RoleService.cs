using Application.Context;
using Domain.Entities;
using eXtensionSharp;
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
        public async Task<TB_ROLE> GetRole(int roleId)
        {
            return await _dbContext.Roles.FirstAsync(m => m.ID == roleId);
        }

        public async Task<IEnumerable<TB_ROLE>> GetRole()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<TB_ROLE> SaveRole(TB_ROLE tbRole)
        {
            TB_ROLE result = null;
            var exists = await _dbContext.Roles.FirstAsync(m => m.ROLE_TYPE == tbRole.ROLE_TYPE);
            if (exists.xIsEmpty())
            {
                //save
                var entityEntry = _dbContext.Roles.Add(tbRole);
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

        public async Task<TB_ROLE> RemoveRole(int id)
        {
            var exists = await _dbContext.Roles.FirstAsync(m => m.ID == id);
            if (exists.xIsEmpty()) throw new Exception("Not found role");

            var resultEntity = _dbContext.Roles.Remove(exists);
            await _dbContext.SaveChangesAsync();
            return resultEntity.Entity;
        }
    }
}