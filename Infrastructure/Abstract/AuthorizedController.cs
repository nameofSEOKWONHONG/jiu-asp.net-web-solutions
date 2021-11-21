using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Abstract
{
    [Authorize]
    public abstract class AuthorizedController<T> : ApiControllerBase<T>
    {
        private ISessionContextService _sessionContextServiceInstance;
        protected ISessionContextService _sessionContextService => _sessionContextServiceInstance ??=
            HttpContext.RequestServices.GetService<ISessionContextService>();
    }
}