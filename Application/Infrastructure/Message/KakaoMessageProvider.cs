using System.Threading.Tasks;
using Application.Request;
using Application.Response;

namespace Application.Infrastructure.Message
{
    public class KakaoMessageProvider : IMessageProvider
    {
        public async Task<IResult> SendMessageAsync(MessageRequestDto request)
        {
            await Task.Delay(1);
            return Result.Success(nameof(EmailMessageProvider));
        }
    }
}