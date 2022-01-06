using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<TB_ROLE_PERMISSION>> GetRolePermissionsAsync();
        Task<TB_ROLE_PERMISSION> GetRolePermissionAsync(int id);
        Task<TB_ROLE_PERMISSION> SavePermissionAsync(TB_ROLE_PERMISSION tbRolePermission);
        Task<TB_ROLE_PERMISSION> RemovePermissionAsync(int id);
    }
}