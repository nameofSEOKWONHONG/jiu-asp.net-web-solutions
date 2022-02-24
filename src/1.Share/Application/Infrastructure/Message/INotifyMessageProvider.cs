using System.Threading.Tasks;
using Application.Request;
using Domain.Response;

namespace Application.Infrastructure.Message
{
    public abstract class NotifyMessageProviderBase : INotifyMessageProvider
    {
        public abstract Task<IResultBase> SendMessageAsync(INotifyMessageRequest request);

        public abstract object ConvertRequest(INotifyMessageRequest request);
    }

    public interface INotifyMessageProvider
    {
        Task<IResultBase> SendMessageAsync(INotifyMessageRequest request);
    }
}