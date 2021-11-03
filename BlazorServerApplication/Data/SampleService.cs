using System;
using Microsoft.AspNetCore.Http;

namespace BlazorServerApplication.Data;

public class SampleService
{
    private readonly IHttpContextAccessor _contextAccessor;
    public SampleService(IHttpContextAccessor contextAccessor)
    {
        this._contextAccessor = contextAccessor;
    }

    public void SampleMethod()
    {
        if (this._contextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("logined");
        }
    }
}