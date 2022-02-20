using Domain.Response;
using eXtensionSharp;
using FluentValidation;
using FluentValidation.Results;
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

        /// <summary>
        /// 수동 model validation (model validation 비활성화하여 사용할 경우 사용)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="result"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected bool TryValidate<TEntity>(TEntity entity, out ActionResult result) 
            where TEntity : class
        {
            if (!this.TryValidateModel(entity))
            {
                var errors = new List<string>();
                ModelState.Values.xForEach(m =>
                {
                    m.Errors.xForEach(m =>
                    {
                        errors.Add(m.ErrorMessage);
                    });
                });
                result = new BadRequestObjectResult(Result.Fail(errors)); 
                return false;
            }

            result = null;
            return true;
        }

        /// <summary>
        /// Fluent Validator (Entity, Dto에 생성자에 정의한 Validator를 사용한다.)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="result"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <returns></returns>
        protected bool TryValidate<TEntity, TValidator>(TEntity entity, out ActionResult result)
            where TEntity : class
            where TValidator : AbstractValidator<TEntity>, new()
        {
            var validator = new TValidator();
            ValidationResult results = validator.Validate(entity);
            
            if (!results.IsValid)
            {
                var errors = new Dictionary<string, List<string>>();
                results.Errors.xForEach(m =>
                {
                    var exists = errors.xFirst(e => e.Key == m.PropertyName);
                    if (exists.Key.xIsNotEmpty())
                    {
                        exists.Value.Add(m.ErrorMessage);
                    }
                    else
                    {
                        errors.Add(m.PropertyName, new List<string>(){m.ErrorMessage});    
                    }
                });
                result = new BadRequestObjectResult(Result<Dictionary<string, List<string>>>.Fail(errors));
                return false;
            }
            result = null;
            return true;
        }
    }
}