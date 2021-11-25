using System.Threading.Tasks;
using Application.Request;
using Application.Response;

namespace Application.Infrastructure.Message
{
    public class SmsMessageProvider : IMessageProvider
    {
        public async Task<IResult> SendMessageAsync(IMessageRequest request)
        {
            
            await Task.Delay(1);
            return Result.Success(nameof(SmsMessageProvider));
        }
    }
}