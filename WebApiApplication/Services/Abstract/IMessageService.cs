using System.Threading.Tasks;
using Application.Request;
using Application.Response;

namespace WebApiApplication.Services.Abstract
{
    public interface IMessageService
    {
        Task<IResult> SendMessageAsync(MessageRequestDto request);
    }
}