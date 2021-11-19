using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Application.Abstract;
using WebApiApplication.Services;
using Application.Enums;
using Application.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MessageController : ApiControllerBase<MessageController>
    {
        private readonly MessageServiceResolver _messageServiceResolver;
        public MessageController(MessageServiceResolver messageServiceResolver)
        {
            this._messageServiceResolver = messageServiceResolver;
        }

        [HttpPost("{messagetype}")]
        public async Task<IActionResult> Send([FromBody]MessageRequestDto request, ENUM_MESSAGE_TYPE messagetype)
        {
            var messageService = this._messageServiceResolver(messagetype);
            var result = await messageService.SendMessageAsync(request);
            return Ok(result);
        }
    }
}