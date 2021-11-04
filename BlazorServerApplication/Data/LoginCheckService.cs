using System;
using Microsoft.AspNetCore.Http;

namespace BlazorServerApplication.Data;

public class LoginCheckService
{
    private readonly IHttpContextAccessor _contextAccessor;
    public LoginCheckService(IHttpContextAccessor contextAccessor)
    {
        this._contextAccessor = contextAccessor;
    }

    public void SampleMethod()
    {
        HttpContext httpContext = this._contextAccessor.HttpContext;
        if (httpContext.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("logined");
        }
    }
}