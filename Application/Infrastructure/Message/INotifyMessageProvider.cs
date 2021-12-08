using System.Threading.Tasks;
using Application.Request;
using Application.Response;

namespace Application.Infrastructure.Message
{
    public abstract class NotifyMessageProviderBase : INotifyMessageProvider
    {
        public abstract Task<IResult> SendMessageAsync(INotifyMessageRequest request);

        public abstract object ConvertRequest(INotifyMessageRequest request);
    }

    public interface INotifyMessageProvider
    {
        Task<IResult> SendMessageAsync(INotifyMessageRequest request);
    }
}