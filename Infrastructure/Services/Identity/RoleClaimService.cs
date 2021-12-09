using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using eXtensionSharp;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly JIUDbContext _context; 
        public RoleClaimService(JIUDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetRoleClaim()
        {
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<RolePermission> GetRoleClaim(int id)
        {
            if (id <= 0) return null;
            return await _context.RolePermissions.FirstAsync(m => m.Id == id);
        }

        public async Task<RolePermission> SaveRoleClaim(RolePermission rolePermission)
        {
            var exists = await GetRoleClaim(rolePermission.Id);
            await exists.xIfEmptyAsync(() =>
            {
                _context.RolePermissions.Add(rolePermission);
                return _context.SaveChangesAsync();
            });
            return rolePermission;
        }

        public async Task<RolePermission> RemoveRoleClaim(int id)
        {
            var exists = await GetRoleClaim(id);
            exists.xIfEmpty(() => throw new KeyNotFoundException());
            
            await exists.xIfNotEmptyAsync(async () =>
            {
                _context.RolePermissions.Remove(exists);
                await _context.SaveChangesAsync();
            });

            return exists;
        }
    }
}