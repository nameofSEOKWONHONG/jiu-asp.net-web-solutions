using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly JIUDbContext _context; 
        public RolePermissionService(JIUDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionsAsync()
        {
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<RolePermission> GetRolePermissionAsync(int id)
        {
            if (id <= 0) return null;
            return await _context.RolePermissions.FirstAsync(m => m.Id == id);
        }

        public async Task<RolePermission> SavePermissionAsync(RolePermission rolePermission)
        {
            var exists = await GetRolePermissionAsync(rolePermission.Id);
            await exists.xIfEmptyAsync(() =>
            {
                _context.RolePermissions.Add(rolePermission);
                return _context.SaveChangesAsync();
            });
            return rolePermission;
        }

        public async Task<RolePermission> RemovePermissionAsync(int id)
        {
            var exists = await GetRolePermissionAsync(id);
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