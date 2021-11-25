using System.Threading.Tasks;
using Application.Request;
using Application.Response;

namespace Application.Infrastructure.Message
{
    public interface IMessageProvider
    {
        Task<IResult> SendMessageAsync(IMessageRequest request);
    }
}