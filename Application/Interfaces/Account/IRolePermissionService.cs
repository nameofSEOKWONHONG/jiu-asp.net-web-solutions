using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermission>> GetRolePermissionsAsync();
        Task<RolePermission> GetRolePermissionAsync(int id);
        Task<RolePermission> SavePermissionAsync(RolePermission rolePermission);
        Task<RolePermission> RemovePermissionAsync(int id);
    }
}