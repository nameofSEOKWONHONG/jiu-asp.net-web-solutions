using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Domain.Entities;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    public interface ISessionContext
    {
        User User { get; set; }
    }

    public class SessionContext : ISessionContext
    {
        public User User { get; set; }
    }

    public class SessionContextService : ISessionContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public SessionContextService(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userService = userService;
        }
        public async Task<ISessionContext> GetSessionAsync()
        {
            var userId = this._httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this._userService.FindUserByIdAsync(Guid.Parse(userId));

            return new SessionContext()
            {
                User = user
            };
        }
    }
}