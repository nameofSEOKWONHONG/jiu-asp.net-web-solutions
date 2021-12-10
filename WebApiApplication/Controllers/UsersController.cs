using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Infrastructure.Abstract;
using Microsoft.Extensions.Logging;
using WebApiApplication.Filters;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [AuthorizeRole(RoleType = "SUPER,ADMIN", PermissionType = "VIEW,CREATE,UPDATE,DELETE")]
    public class UsersController : AuthorizedController<UsersController>
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("GetAll/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAll(string searchCol = "", string searchValue = "", int pageIndex = 1, int pageSize = 10)
        {
            var user = this.SessionContext.User;
            var user2 = this.SessionContext.User;
            var user3 = this.SessionContext.User;
            _logger.LogInformation("test");
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
        public async Task<IActionResult> CreateUser(User userData)
        {
            if (!this.TryValidate(userData, out ActionResult result)) return result;
            return Ok(await this.userService.CreateUserAsync(userData));
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User userData)
        {
            if (!this.TryValidate(userData, out ActionResult result)) return result;
            return Ok(await this.userService.UpdateUserAsync(userData));
        }

        [HttpDelete("RemoveUser")]
        public async Task<IActionResult> RemoveUser(Guid userId, string email)
        {
            return Ok(await this.userService.RemoveUserAsync(userId, email));
        }
    }
}