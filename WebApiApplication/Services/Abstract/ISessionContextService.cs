using System;

namespace WebApiApplication.Services.Abstract
{
    public interface ISessionContextService
    {
        ISessionContext GetUser(Guid id);
    }
}