using Application.Context;
using Domain.Entities;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Account
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly ApplicationDbContext _context; 
        public RolePermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TB_ROLE_PERMISSION>> GetRolePermissionsAsync()
        {
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<TB_ROLE_PERMISSION> GetRolePermissionAsync(int id)
        {
            if (id <= 0) return null;
            return await _context.RolePermissions.FirstAsync(m => m.ID == id);
        }

        public async Task<TB_ROLE_PERMISSION> SavePermissionAsync(TB_ROLE_PERMISSION tbRolePermission)
        {
            var exists = await GetRolePermissionAsync(tbRolePermission.ID);
            await exists.xIfEmptyAsync(() =>
            {
                _context.RolePermissions.Add(tbRolePermission);
                return _context.SaveChangesAsync();
            });
            return tbRolePermission;
        }

        public async Task<TB_ROLE_PERMISSION> RemovePermissionAsync(int id)
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