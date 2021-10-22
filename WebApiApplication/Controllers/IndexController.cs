using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    public class IndexController : ApiControllerBase
    {
        public IndexController(ILogger<IndexController> logger) : base(logger)
        {
            
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Hello, The purpose of this project is to create a template similar to \"typescript-express-starter\"");
            sb.AppendLine("(https://github.com/ljlm0402/typescript-express-starter)");
            sb.AppendLine("git clone [project site] [your project name]");
            sb.AppendLine("cd [your project name]");
            sb.AppendLine("dotnet restore");
            sb.AppendLine("dotnet run or dotnet watch run");
            return Ok(sb.ToString());
        }
    }
}