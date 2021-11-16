﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SharedLibrary.Entities;

namespace WebApiApplication.Services.Abstract
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
}