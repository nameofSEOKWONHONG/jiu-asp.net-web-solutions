using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services;
using Application.Dtos;
using Application.Entities;
using Application.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController : ApiControllerBase<AuthController>
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        
        public AuthController(IUserService userService,
            IAuthService authService
            )
        {
            this._userService = userService;
            this._authService = authService;

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
            var token = await _authService.Login(user);
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