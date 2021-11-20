using System.Threading.Tasks;
using Domain.Entities;
using Application.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IAuthService
    {
        Task<string> Login(User user);
    }
}