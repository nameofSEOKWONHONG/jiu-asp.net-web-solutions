using Microsoft.AspNetCore.Http;

namespace ClientApplication.Services;

public class LoginCheckService
{
    private readonly IHttpContextAccessor _contextAccessor;
    public LoginCheckService(IHttpContextAccessor contextAccessor)
    {
        this._contextAccessor = contextAccessor;
    }

    public void SampleMethod()
    {
        var userIdentity = this._contextAccessor.HttpContext.User.Identity;
        if (userIdentity != null && userIdentity.IsAuthenticated)
        {
            Console.WriteLine("logined");
        }
    }
}   