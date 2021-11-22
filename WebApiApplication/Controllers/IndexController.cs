using System.Text;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using Infrastructure.Abstract;
using WebApiApplication.Services;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    [ApiVersion("2")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class IndexController : VersionApiController<IndexController>
    {
        private readonly VehicleCreatorServiceFactory _vehicleCreatorServiceFactory;
        private readonly IMessageProvider _messageProvider;
        public IndexController(VehicleCreatorServiceFactory vehicleCreatorServiceFactory,
            MessageProviderResolver messageServiceResolver)
        {
            this._vehicleCreatorServiceFactory = vehicleCreatorServiceFactory;
            this._messageProvider = messageServiceResolver(ENUM_MESSAGE_TYPE.EMAIL);
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Hello, The purpose of this project is to create a template similar to \"typescript-express-starter\"");
                sb.AppendLine("(https://github.com/ljlm0402/typescript-express-starter)");
                sb.AppendLine("git clone [project site] [your project name]");
                sb.AppendLine("cd [your project name]");
                sb.AppendLine("dotnet restore");
                sb.AppendLine("dotnet run or dotnet watch run");
                return Ok(sb.ToString());
            });
        }
    }
}