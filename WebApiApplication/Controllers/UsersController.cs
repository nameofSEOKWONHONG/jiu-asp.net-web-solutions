using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services;
using Application.Entities;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    public class UsersController : AuthorizedApiController<UsersController>
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await this._sessionContextService.GetSessionAsync();
            return Ok(await this.userService.FindAllUserAsync());
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            return Ok(await this.userService.FindUserByIdAsync(userId));
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            return Ok(await this.userService.FindUserByEmailAsync(email));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User userData)
        {
            if (!this.TryValidate(userData, out ActionResult result)) return result;
            return Ok(await this.userService.CreateUserAsync(userData));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User userData)
        {
            if (!this.TryValidate(userData, out ActionResult result)) return result;
            return Ok(await this.userService.UpdateUserAsync(userData));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Guid userId, string email)
        {
            return Ok(await this.userService.DeleteUserAsync(userId, email));
        }
    }
}