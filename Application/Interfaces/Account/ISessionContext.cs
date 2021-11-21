using Domain.Entities;

namespace WebApiApplication.Services.Abstract
{
    public interface ISessionContext
    {
        User User { get; set; }
    }
}