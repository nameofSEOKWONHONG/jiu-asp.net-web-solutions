using System;
using System.Threading.Tasks;

namespace WebApiApplication.Services.Abstract
{
    public interface ISessionContextService
    {
        Task<ISessionContext> GetSessionAsync();
    }
}