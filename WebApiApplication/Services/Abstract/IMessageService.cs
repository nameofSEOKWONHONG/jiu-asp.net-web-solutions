using System.Threading.Tasks;
using SharedLibrary.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(MessageRequestDto request);
    }
}