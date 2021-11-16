namespace WebApiApplication.Services.Abstract
{
    public interface ISessionContextService
    {
        ISessionContext GetUser(int id);
    }
}