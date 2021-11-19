using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Controllers
{
    public class GenerateViewController : ApiControllerBase<GenerateViewController>
    {
        private readonly IGenerateViewService _generateViewService;
        public GenerateViewController(IGenerateViewService generateViewService)
        {
            this._generateViewService = generateViewService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_generateViewService.GetForm(""));
        }
    }
}