using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Infrastructure.Abstract.Controllers;
using WebApiApplication.Filters;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [RoleAuthorize(RoleType = "SUPER,ADMIN", PermissionType = "VIEW,CREATE,UPDATE,DELETE")]
    public class UsersController : SessionController<UsersController>
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("GetAll/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAll(string searchCol = "", string searchValue = "", int pageIndex = 1, int pageSize = 10)
        {
            var result = await this.userService.FindAllUserAsync(searchCol, searchValue, pageIndex, pageSize); 
            return Ok(result);
        }

        [HttpGet("Get/{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            return Ok(await this.userService.FindUserByIdAsync(userId));
        }

        [HttpGet("{userId}/Get/{email}")]
        public async Task<IActionResult> Get(Guid userId, string email)
        {
            var user = await this.userService.FindUserByIdAsync(userId);
            if (user == null) return new NotFoundResult();
            var result = await this.userService.FindUserByEmailAsync(email); 
            return Ok(result);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(TB_USER tbUserData)
        {
            if (!this.TryValidate(tbUserData, out ActionResult result)) return result;
            return Ok(await this.userService.CreateUserAsync(tbUserData));
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(TB_USER tbUserData)
        {
            if (!this.TryValidate(tbUserData, out ActionResult result)) return result;
            return Ok(await this.userService.UpdateUserAsync(tbUserData));
        }

        [HttpDelete("RemoveUser")]
        public async Task<IActionResult> RemoveUser(Guid userId, string email)
        {
            return Ok(await this.userService.RemoveUserAsync(userId, email));
        }
    }
}