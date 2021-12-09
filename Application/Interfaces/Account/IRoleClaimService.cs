using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRoleClaimService
    {
        Task<IEnumerable<RolePermission>> GetRoleClaim();
        Task<RolePermission> GetRoleClaim(int id);
        Task<RolePermission> SaveRoleClaim(RolePermission rolePermission);
        Task<RolePermission> RemoveRoleClaim(int id);
    }
}