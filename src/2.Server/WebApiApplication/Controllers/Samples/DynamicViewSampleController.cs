// using System.Threading.Tasks;
// using Infrastructure.Abstract;
// using Microsoft.AspNetCore.Mvc;
// using WebApiApplication.Services.Abstract;
//
// namespace WebApiApplication.Controllers
// {
//     public class GenerateViewController : ApiControllerBase<GenerateViewController>
//     {
//         private readonly IGenerateViewService _generateViewService;
//         public GenerateViewController(IGenerateViewService generateViewService)
//         {
//             this._generateViewService = generateViewService;
//         }
//
//         [HttpGet]
//         public IActionResult Get()
//         {
//             return Ok(_generateViewService.GetForm(""));
//         }
//     }
// }