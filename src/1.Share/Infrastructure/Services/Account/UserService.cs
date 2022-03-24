using System.Linq.Expressions;
using Application.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Account
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;
        
        public UserService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<IEnumerable<TB_USER>> FindAllUserAsync(string searchCol = "", string searchVal = "", int pageIndex = 1, int pageSize = 10)
        {
            if(searchCol.xIsNotEmpty())
            {
                var filter = ExpressionUtils.BuildPredicate<TB_USER>(searchCol, "Contains", searchVal);
                return await dbContext.Users
                    .Where(filter)
                    .Include(m => m.ROLE)
                    .Include(m => m.ROLE.ROLE_PERMISSION)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            
            return await dbContext.Users
                .Include(m => m.ROLE)
                .Include(m => m.ROLE.ROLE_PERMISSION)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TB_USER>> FindAllUserByRoleAsync(ENUM_ROLE_TYPE[] roleTypes)
        {
            return await dbContext.Users.Include(m => m.ROLE)
                .Where(m => roleTypes.Contains(m.ROLE.ROLE_TYPE))
                .ToListAsync();
        }

        public async Task<TB_USER> FindUserByIdAsync(Guid userId)
        {
            return await dbContext.Users
                .Include(m => m.ROLE)
                .Include(m => m.ROLE.ROLE_PERMISSION)
                .FirstOrDefaultAsync(m => m.ID == userId);
        }

        public async Task<TB_USER> FindUserByEmailAsync(string email)
        {
            return await dbContext.Users
                .Include(m => m.ROLE)
                .Include(m => m.ROLE.ROLE_PERMISSION)
                .FirstOrDefaultAsync(m => m.EMAIL == email);
        }

        public async Task<bool> ExistsSuperUserAsync()
        {
            var exists = await dbContext.Users
                .Include(m => m.ROLE)
                .Take(1)
                .FirstOrDefaultAsync(m => m.ROLE.ROLE_TYPE == ENUM_ROLE_TYPE.SUPER);
            return exists.xIsNotEmpty();
        }

        public async Task<TB_USER> CreateUserAsync(TB_USER tbUser)
        {
            #region [user check]

            var exists = await FindUserByEmailAsync(tbUser.EMAIL);
            if (exists != null) throw new Exception("already exists.");

            if (tbUser.ROLE.ROLE_TYPE == ENUM_ROLE_TYPE.SUPER)
            {
                var superUser = await ExistsSuperUserAsync();
                if (superUser)
                {
                    throw new Exception("Super user is already exists.");
                }
            }

            #endregion

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbUser.PASSWORD);
            if (BCrypt.Net.BCrypt.Verify(tbUser.PASSWORD, hashedPassword))
            {
                tbUser.PASSWORD = hashedPassword;
            }
            tbUser.ID = Guid.NewGuid();
            tbUser.WRITE_ID = tbUser.ID.ToString();
            tbUser.WRITE_DT = DateTime.UtcNow;
            var result = await dbContext.Users.AddAsync(tbUser);
            await dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<TB_USER> UpdateUserAsync(TB_USER tbUserData)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.EMAIL == tbUserData.EMAIL);
            if (exists == null) throw new Exception("not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tbUserData.PASSWORD);
            if (BCrypt.Net.BCrypt.Verify(tbUserData.PASSWORD, hashedPassword))
            {
                exists.PASSWORD = hashedPassword;
            }

            await dbContext.SaveChangesAsync();
            return exists;
        }

        public async Task<TB_USER> RemoveUserAsync(Guid userId, string email)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.ID == userId && m.EMAIL == email);
            if (exists == null) throw new Exception("not found");

            dbContext.Users.Remove(exists);

            await dbContext.SaveChangesAsync();

            return exists;
        }
    }
    
    
}