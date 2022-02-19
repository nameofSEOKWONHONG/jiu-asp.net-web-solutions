using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Abstract
{
    [Authorize]
    public abstract class SessionController<TController> : VersionController<TController>
    {
        private ISessionContext _sessionContextInstance;
        protected ISessionContext SessionContext => _sessionContextInstance ??=
            HttpContext.RequestServices.GetRequiredService<ISessionContextService>()
                .GetSessionAsync()
                .GetAwaiter()
                .GetResult();
    }
}