using SharedLibrary.Entities;
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
        private readonly IUserService _userService;
        public SessionContextService(IUserService userService)
        {
            this._userService = userService;
        }
        public ISessionContext GetUser(int id)
        {
            var user = this._userService.FindUserByIdAsync(id).GetAwaiter().GetResult();

            return new SessionContext()
            {
                User = user
            };
        }
    }
}