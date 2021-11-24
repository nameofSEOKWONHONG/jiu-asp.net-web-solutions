using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<User>> FindAllUserAsync();
        Task<User> FindUserByIdAsync(Guid userId);
        Task<User> FindUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User userData);
        Task<User> RemoveUserAsync(Guid userId, string email);
    }
}