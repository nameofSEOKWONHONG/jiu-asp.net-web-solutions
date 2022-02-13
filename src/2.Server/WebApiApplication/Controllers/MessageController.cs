using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Infrastructure.Message;
using Domain.Enums;
using Application.Request;
using Domain.Entities;
using Infrastructure.Abstract;
using WebApiApplication.Filters;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MessageController : AuthorizedController<MessageController>
    {
        private readonly MessageProviderResolver _messageProviderResolver;
        public MessageController(MessageProviderResolver messageProviderResolver)
        {
            this._messageProviderResolver = messageProviderResolver;
        }

        [HttpPost("SendEMail")]
        public async Task<IActionResult> SendEMail(EmailNotifyMessageRequest request)
        {
            var messageProvider = this._messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL);
            var result = await messageProvider.SendMessageAsync(request);
            return Ok(result);
        }

        [HttpPost("SendSms")]
        public async Task<IActionResult> SendSms(INotifyMessageRequest request)
        {
            var messageProvider = this._messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.SMS);
            var result = await messageProvider.SendMessageAsync(request);
            return Ok(result);
        }

        [HttpPost("SendKakao")]
        public async Task<IActionResult> SendKakao(INotifyMessageRequest request)
        {
            var messageProvider = this._messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.KAKAO);
            var result = await messageProvider.SendMessageAsync(request);
            return Ok(result);
        }
    }
}