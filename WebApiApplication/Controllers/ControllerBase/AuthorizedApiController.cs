﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [Authorize]
    public abstract class AuthorizedApiController<T> : ApiControllerBase<T>
    {
        private ISessionContextService _sessionContextServiceInstance;
        protected ISessionContextService _sessionContextService => _sessionContextServiceInstance ??=
            HttpContext.RequestServices.GetService<ISessionContextService>();
    }
}