using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace WebApiApplication.Services.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<TB_USER>> FindAllUserAsync(string searchCol = "", string searchVal = "", int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<TB_USER>> FindAllUserByRoleAsync(ENUM_ROLE_TYPE[] roleTypes);
        Task<TB_USER> FindUserByIdAsync(Guid userId);
        Task<TB_USER> FindUserByEmailAsync(string email);
        Task<bool> ExistsSuperUserAsync();
        Task<TB_USER> CreateUserAsync(TB_USER tbUser);
        Task<TB_USER> UpdateUserAsync(TB_USER tbUserData);
        Task<TB_USER> RemoveUserAsync(Guid userId, string email);
    }
}