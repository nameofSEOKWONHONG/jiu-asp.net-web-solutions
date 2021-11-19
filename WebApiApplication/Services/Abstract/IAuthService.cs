using System.Threading.Tasks;
using SharedLibrary.Entities;
using SharedLibrary.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IAuthService
    {
        Task<string> Login(User user);
    }
}