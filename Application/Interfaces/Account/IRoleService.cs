using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRoleService
    {
        Task<IEnumerable<TB_ROLE>> GetRole();
        Task<TB_ROLE> GetRole(int id);
        Task<TB_ROLE> SaveRole(TB_ROLE tbRole);
        Task<TB_ROLE> RemoveRole(int id);
    }
}