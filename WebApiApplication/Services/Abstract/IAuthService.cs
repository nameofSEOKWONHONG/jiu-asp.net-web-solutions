using System.Threading.Tasks;
using SharedLibrary.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IAuthService
    {
        Task<string> Login(RegisterRequest registerRequest);
    }
}