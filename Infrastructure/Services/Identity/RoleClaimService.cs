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

        public async Task<IEnumerable<RoleClaim>> GetRoleClaim()
        {
            return await _context.RoleClaims.ToListAsync();
        }

        public async Task<RoleClaim> GetRoleClaim(int id)
        {
            if (id <= 0) return null;
            return await _context.RoleClaims.FirstAsync(m => m.Id == id);
        }

        public async Task<RoleClaim> SaveRoleClaim(RoleClaim roleClaim)
        {
            var exists = await GetRoleClaim(roleClaim.Id);
            await exists.xIfEmptyAsync(() =>
            {
                _context.RoleClaims.Add(roleClaim);
                return _context.SaveChangesAsync();
            });
            return roleClaim;
        }

        public async Task<RoleClaim> RemoveRoleClaim(int id)
        {
            var exists = await GetRoleClaim(id);
            exists.xIfEmpty(() => throw new KeyNotFoundException());
            
            await exists.xIfNotEmptyAsync(async () =>
            {
                _context.RoleClaims.Remove(exists);
                await _context.SaveChangesAsync();
            });

            return exists;
        }
    }
}