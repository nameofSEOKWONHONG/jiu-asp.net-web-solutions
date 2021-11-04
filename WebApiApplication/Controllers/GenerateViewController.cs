using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Controllers
{
    public class GenerateViewController : ApiControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGenerateViewService _generateViewService;
        public GenerateViewController(ILogger<GenerateViewController> logger,
            IGenerateViewService generateViewService) : base(logger)
        {
            this._logger = logger;
            this._generateViewService = generateViewService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_generateViewService.GetForm(""));
        }
    }

    public interface IGenerateViewService
    {
        string GetForm(string formId);
    }

    public class GenerateViewService : IGenerateViewService
    {
        public string GetForm(string formId)
        {
            return "<button type=\"button\" class=\"mud-button-root mud-button mud-button-filled mud-button-filled-primary mud-button-filled-size-medium mud-ripple\"><span class=\"mud-button-label\">ClickMe</span></button>";
        }
    }
}