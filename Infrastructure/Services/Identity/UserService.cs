using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Context;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly JUIDbContext dbContext;
        public UserService(JUIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<User>> FindAllUserAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User> FindUserByIdAsync(Guid userId)
        {
            return await dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (exists != null) throw new Exception("already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            if (BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword))
            {
                user.Password = hashedPassword;
            }
            user.Id = Guid.NewGuid();
            user.WriteId = user.Id;
            user.WriteDt = DateTime.UtcNow;
            var result = await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<User> UpdateUserAsync(User userData)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Email == userData.Email);
            if (exists == null) throw new Exception("not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userData.Password);
            if (BCrypt.Net.BCrypt.Verify(userData.Password, hashedPassword))
            {
                exists.Password = hashedPassword;
            }

            await dbContext.SaveChangesAsync();
            return exists;
        }

        public async Task<User> DeleteUserAsync(Guid userId, string email)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId && m.Email == email);
            if (exists == null) throw new Exception("not found");

            dbContext.Users.Remove(exists);

            await dbContext.SaveChangesAsync();

            return exists;
        }
    }
}