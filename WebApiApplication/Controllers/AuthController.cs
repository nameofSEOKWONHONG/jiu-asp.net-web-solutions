using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services;
using WebApiApplication.SharedLibrary.Dtos;
using WebApiApplication.SharedLibrary.Entities;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController : ApiControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        
        public AuthController(ILogger<AuthController> logger,
            IUserService userService,
            IAuthService authService
            ) : base(logger)
        {
            this.userService = userService;
            this.authService = authService;

        }
        
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserRequest userRequest)
        {
            if (!this.TryValidate(userRequest, out ActionResult result))
            {
                return result;
            }
            var token = await authService.Login(userRequest);
            if (!string.IsNullOrEmpty(token)) return Ok(token);
            return NotFound("not matched");
        }

        [AllowAnonymous]
        [HttpPost("SingUp")]
        public async Task<IActionResult> SignUp(UserRequest userRequest)
        {
            if (!this.TryValidate(userRequest, out ActionResult result))
            {
                return result;
            }

            return Ok(await this.userService.CreateUserAsync(userRequest.Adapt<User>()));
        }
    }
}