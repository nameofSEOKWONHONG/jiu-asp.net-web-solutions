using System.Threading.Tasks;
using SharedLibrary.Request;
using SharedLibrary.Response;

namespace WebApiApplication.Services.Abstract
{
    public interface IMessageService
    {
        Task<IResult> SendMessageAsync(MessageRequestDto request);
    }
}