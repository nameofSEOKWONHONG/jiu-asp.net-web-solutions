using Application.Interfaces;
using eXtensionSharp;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Abstract
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase<TController> : ControllerBase
    {
        /*
         * 생성자 의존성 주입으로 IServiceProvider를 받지 않는다.
         * 위 행위가 필요하다면 HttpContext.RequestService.GetService<>();를 사용한다.
         * 또한 HttpContext.RequestService로 의존성 주입을 할때 해당 개체의 인스턴스가 Scope라면
         * 같은 인스턴스 Scope인지 확인해야 하고 같은 Scope를 강제해야 한다며 ScopeCreate를 사용하도록 한다.
         * IServiceProvider 의존성 주입 방식은 권장되지 않는 방식.
         */
        private ILogger<TController> _loggerInstance;
        private IMediator _mediatorInstance;
        /// <summary>
        /// httpcontext로 의존성 주입
        /// </summary>
        protected ILogger<TController> _logger => _loggerInstance ??= HttpContext.RequestServices.GetRequiredService<ILogger<TController>>();
        /// <summary>
        /// httpcontext로 의존성 주입 (CQRS)
        /// </summary>
        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected bool TryValidate<TEntity>(TEntity model, out ActionResult result) where TEntity : class
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