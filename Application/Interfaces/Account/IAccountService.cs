using System.Threading.Tasks;
using Domain.Entities;
using Application.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IAccountService
    {
        Task<string> Login(User user);
    }
}