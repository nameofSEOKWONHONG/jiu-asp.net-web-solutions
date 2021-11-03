using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using WebApiApplication.Dtos;
using WebApiApplication.Services;

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
        public async Task<ResultBase> Send([FromBody]MessageRequestDto request, ENUM_MESSAGE_TYPE messagetype)
        {
            ResultBase result = new ResultBase();
            
            var service = this._messageServiceFactory.CreateService(messagetype);
            result.Success = await service.SendMessageAsync(request);

            return result;
        }
    }
}