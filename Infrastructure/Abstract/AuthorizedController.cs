using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Abstract
{
    [Authorize]
    public abstract class AuthorizedController<TController> : VersionController<TController>
    {
        private ISessionContextService _sessionContextServiceInstance;
        protected ISessionContextService _sessionContextService => _sessionContextServiceInstance ??=
            HttpContext.RequestServices.GetService<ISessionContextService>();
    }
}