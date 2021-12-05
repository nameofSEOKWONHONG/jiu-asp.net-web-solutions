using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRole();
        Task<Role> GetRole(int id);
        Task<Role> SaveRole(Role role);
        Task<Role> RemoveRole(int id);
    }
}