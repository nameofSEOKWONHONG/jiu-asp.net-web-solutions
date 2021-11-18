using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using SharedLibrary.Abstract;
using WebApiApplication.Services;
using SharedLibrary.Enums;
using SharedLibrary.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MessageController : ApiControllerBase<MessageController>
    {
        private readonly IMessageService _messageService;
        public MessageController(MessageServiceResolver messageServiceResolver)
        {
            this._messageService = messageServiceResolver(ENUM_MESSAGE_TYPE.SMS);
        }

        [HttpPost("{messagetype}")]
        public async Task<ResponseResultBase> Send([FromBody]MessageRequestDto request, ENUM_MESSAGE_TYPE messagetype)
        {
            ResponseResultBase responseResult = new ResponseResultBase();
            
            responseResult.Success = await _messageService.SendMessageAsync(request);

            return responseResult;
        }
    }
}