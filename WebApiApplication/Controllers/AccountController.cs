using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Request;
using Infrastructure.Abstract;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AccountController : VersionController<AccountController>
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        
        public AccountController(IUserService userService,
            IAccountService accountService
            )
        {
            this._userService = userService;
            this._accountService = accountService;

        }
        
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInRequest signInRequest)
        {
            if (!this.TryValidate(signInRequest, out ActionResult result))
            {
                return result;
            }

            var user = signInRequest.Adapt<User>();
            var token = await _accountService.Login(user);
            if (!string.IsNullOrEmpty(token)) return Ok(token);
            return NotFound("not matched");
        }

        [AllowAnonymous]
        [HttpPost("SingUp")]
        public async Task<IActionResult> SignUp(RegisterRequest registerRequest)
        {
            if (!this.TryValidate(registerRequest, out ActionResult result))
            {
                return result;
            }

            return Ok(await this._userService.CreateUserAsync(registerRequest.Adapt<User>()));
        }
    }
}