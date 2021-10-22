using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Entities;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.DataContext;

namespace WebApiApplication.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> FindAllUserAsync();
        Task<User> FindUserByIdAsync(int userId);
        Task<User> FindUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User userData);
        Task<User> UpdateUserAsync(User userData);
        Task<User> DeleteUserAsync(int userId, string email);
    }

    public class UserService : IUserService
    {
        private readonly AccountDbContext dbContext;
        public UserService(AccountDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<User>> FindAllUserAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User> FindUserByIdAsync(int userId)
        {
            return await dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<User> CreateUserAsync(User userData)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Email == userData.Email);
            if (exists != null) throw new Exception("already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userData.Password);
            if (BCrypt.Net.BCrypt.Verify(userData.Password, hashedPassword))
            {
                userData.Password = hashedPassword;
            }

            var result = await dbContext.Users.AddAsync(userData);
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

        public async Task<User> DeleteUserAsync(int userId, string email)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId && m.Email == email);
            if (exists == null) throw new Exception("not found");

            dbContext.Users.Remove(exists);

            await dbContext.SaveChangesAsync();

            return exists;
        }
    }

    /// <summary>
    /// manual user service implement
    /// </summary>
    // public class UserService : IUserService
    // {
    //     //password : q1w2e3r4
    //     private readonly IList<User> users = new List<User>()
    //     {
    //         new User() { Id = 1, Email = "lim@gmail.com",  Password = "$2a$10$M5S9uemZ3LITgDtKP1Mq4OdROUbLWItM4y/AMcYxLXgIzbMTsAPkK"},
    //         new User() { Id = 2, Email = "kim@gmail.com",  Password = "$2a$10$M5S9uemZ3LITgDtKP1Mq4OdROUbLWItM4y/AMcYxLXgIzbMTsAPkK" },
    //         new User() { Id = 3, Email = "park@gmail.com", Password = "$2a$10$M5S9uemZ3LITgDtKP1Mq4OdROUbLWItM4y/AMcYxLXgIzbMTsAPkK" },
    //         new User() { Id = 4, Email = "choi@gmail.com", Password = "$2a$10$M5S9uemZ3LITgDtKP1Mq4OdROUbLWItM4y/AMcYxLXgIzbMTsAPkK" },
    //     };
    //     
    //     public async Task<IEnumerable<User>> FindAllUserAsync()
    //     {
    //         return await Task.Run(() => this.users);
    //     }
    //
    //     public async Task<User> FindUserByIdAsync(int userId)
    //     {
    //         return await Task.Run(() => this.users.First(m => m.Id == userId));
    //     }
    //
    //     public async Task<User> FindUserByEmailAsync(string email)
    //     {
    //         return await Task.Run(() => this.users.First(m => m.Email == email));
    //     }
    //
    //     public async Task<User> CreateUserAsync(User userData)
    //     {
    //         if (userData == null) throw new BadHttpRequestException("You're not userData", 400);
    //         
    //         var existsUser = this.users.FirstOrDefault(m => m.Email == userData.Email);
    //         if (existsUser != null)
    //         {
    //             throw new BadHttpRequestException($"Your email {existsUser.Email} already exists", 409);
    //         }
    //
    //         var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userData.Email, 10);
    //         var createUserData = new User()
    //             {Id = this.users.Count + 1, Email = userData.Email, Password = hashedPassword};
    //         this.users.Add(createUserData);
    //         
    //         return createUserData;
    //     }
    //
    //     public async Task<User> UpdateUserAsync(User userData)
    //     {
    //         if (userData == null) throw new BadHttpRequestException("You're not userData", 400);
    //
    //         var existsUser = this.users.FirstOrDefault(m => m.Id == userData.Id);
    //         if (existsUser == null) throw new BadHttpRequestException("You're not user", 409);
    //
    //         var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userData.Password, 10);
    //         existsUser.Password = hashedPassword;
    //         existsUser.Email = userData.Email;
    //
    //         return existsUser;
    //     }
    //
    //     public async Task<User> DeleteUserAsync(int userId)
    //     {
    //         var existsUser = this.users.FirstOrDefault(m => m.Id == userId);
    //         if (existsUser == null) throw new BadHttpRequestException("You're not user", 409);
    //
    //         this.users.Remove(existsUser);
    //         return existsUser;
    //     }
    // }
}