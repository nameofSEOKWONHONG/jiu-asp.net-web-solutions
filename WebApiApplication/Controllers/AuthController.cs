using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services;
using SharedLibrary.Dtos;
using SharedLibrary.Entities;
using SharedLibrary.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController : ApiControllerBase<AuthController>
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        
        public AuthController(IUserService userService,
            IAuthService authService
            )
        {
            this.userService = userService;
            this.authService = authService;

        }
        
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(RegisterRequest registerRequest)
        {
            if (!this.TryValidate(registerRequest, out ActionResult result))
            {
                return result;
            }
            var token = await authService.Login(registerRequest);
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

            return Ok(await this.userService.CreateUserAsync(registerRequest.Adapt<User>()));
        }
    }
}