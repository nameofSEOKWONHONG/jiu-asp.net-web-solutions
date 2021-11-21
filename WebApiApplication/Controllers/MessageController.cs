using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Infrastructure.Message;
using Domain.Enums;
using Application.Request;
using Infrastructure.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MessageController : ApiControllerBase<MessageController>
    {
        private readonly MessageProviderResolver _messageProviderResolver;
        public MessageController(MessageProviderResolver messageProviderResolver)
        {
            this._messageProviderResolver = messageProviderResolver;
        }

        [HttpPost("{messagetype}")]
        public async Task<IActionResult> Send([FromBody]MessageRequestDto request, ENUM_MESSAGE_TYPE messagetype)
        {
            var messageProvider = this._messageProviderResolver(messagetype);
            var result = await messageProvider.SendMessageAsync(request);
            return Ok(result);
        }
    }
}