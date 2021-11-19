using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ApiControllerBase<T> : ControllerBase
    {
        private ILogger<T> _loggerInstance;
        private IMediator _mediatorInstance;
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();

        
        protected ISessionContext SessionContext
        {
            get
            {
                //httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
                var userId = this.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                var userService = HttpContext.RequestServices.GetService<IUserService>();
                var user = userService.FindUserByIdAsync(Guid.Parse(userId)).GetAwaiter().GetResult();
                return new SessionContext() {User = user};
            }
        }
        
        
        protected bool TryValidate<T>(T model, out ActionResult result) where T : class
        {
            if (!this.TryValidateModel(model))
            {
                result = ValidationProblem(ModelState);
                return false;
            }

            result = null;
            return true;
        }
    }
}