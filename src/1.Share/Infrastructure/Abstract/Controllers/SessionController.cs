using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Abstract.Controllers
{
    [Authorize]
    public abstract class SessionController<TController> : VersionController<TController>
    {
        private ISessionContext _sessionContextInstance;

        protected ISessionContext SessionContext
        {
            get
            {
                if (_sessionContextInstance != null) return _sessionContextInstance;
                _sessionContextInstance = HttpContext.RequestServices.GetRequiredService<ISessionContextService>()
                    .GetSession();
                return _sessionContextInstance;
            }
            
        } 
    }
}