using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Abstract
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase<T> : ControllerBase
    {
        private ILogger<T> _loggerInstance;
        private IMediator _mediatorInstance;
        /// <summary>
        /// httpcontext로 의존성 주입
        /// </summary>
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        /// <summary>
        /// httpcontext로 의존성 주입 (CQRS)
        /// </summary>
        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
        
        protected bool TryValidate<T>(T model, out ActionResult result) where T : class
        {
            if (!this.TryValidateModel(model))
            {
                result = ValidationProblem(ModelState);
                return false;
            }

            result = null;
            return true;
        }
    }
}