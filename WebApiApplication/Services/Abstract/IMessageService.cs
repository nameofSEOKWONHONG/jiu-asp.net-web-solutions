using System.Threading.Tasks;
using SharedLibrary.Request;

namespace WebApiApplication.Services.Abstract
{
    public interface IMessageService
    {
        Task<MessageResult> SendMessageAsync(MessageRequestDto request);
    }

    public class MessageResult
    {
        public string Name { get; set; }
        public bool Success { get; set; }
    }
}