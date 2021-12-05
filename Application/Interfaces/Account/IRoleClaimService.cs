using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IRoleClaimService
    {
        Task<IEnumerable<RoleClaim>> GetRoleClaim();
        Task<RoleClaim> GetRoleClaim(int id);
        Task<RoleClaim> SaveRoleClaim(RoleClaim roleClaim);
        Task<RoleClaim> RemoveRoleClaim(int id);
    }
}