using System;
using Microsoft.AspNetCore.Http;

namespace ClientApplication.Service
{
    public class LoginCheckService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public LoginCheckService(IHttpContextAccessor contextAccessor)
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
}

