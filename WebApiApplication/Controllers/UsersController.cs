using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Infrastructure.Abstract;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    public class UsersController : AuthorizedController<UsersController>
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }
        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await this.userService.FindAllUserAsync());
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