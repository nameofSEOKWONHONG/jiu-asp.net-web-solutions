using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using SharedLibrary.Abstract;
using WebApiApplication.Services;
using SharedLibrary.Enums;
using SharedLibrary.Request;

namespace WebApiApplication.Controllers
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MessageController : ApiControllerBase
    {
        private readonly MessageServiceFactory _messageServiceFactory;
        public MessageController(ILogger<MessageController> logger,
            MessageServiceFactory messageServiceFactory) : base(logger)
        {
            this._messageServiceFactory = messageServiceFactory;
        }

        [HttpPost("{messagetype}")]
        public async Task<ResponseResultBase> Send([FromBody]MessageRequestDto request, ENUM_MESSAGE_TYPE messagetype)
        {
            ResponseResultBase responseResult = new ResponseResultBase();
            
            var service = this._messageServiceFactory.CreateService(messagetype);
            responseResult.Success = await service.SendMessageAsync(request);

            return responseResult;
        }
    }
}