using System.Transactions;
using Application.Abstract;
using Application.Infrastructure.Validation;
using Domain.Response;
using eXtensionSharp;
using FluentValidation;
using InjectionExtension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Abstract.Controllers
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
                result = new BadRequestObjectResult(ResultBase.Fail(errors)); 
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
        protected (bool IsValid, DynamicDictionary<List<string>> result) TryValidate<TEntity, TValidator>(TEntity entity)
            where TEntity : class
            where TValidator : AbstractValidator<TEntity>, new()
        {
            return ValidatorCore.TryValidate<TEntity, TValidator>(entity);
        }

        protected async Task<(bool IsValid, DynamicDictionary<List<string>> result)> TryValidateAsync<TEntity, TValidator>(TEntity entity)
            where TEntity : class
            where TValidator : AbstractValidator<TEntity>, new()
        {
            return await ValidatorCore.TryValidateAsync<TEntity, TValidator>(entity);
        }

        protected IActionResult ResultOk<TEntity>(TEntity entity) => Ok(ResultBase<TEntity>.Success(entity));

        protected IActionResult ResultFail<TEntity>(TEntity entity) => Ok(ResultBase<TEntity>.Fail(entity));
        
        protected async Task<IActionResult> ResultOkAsync<TEntity>(TEntity entity) => Ok(await ResultBase<TEntity>.SuccessAsync(entity));

        protected async Task<IActionResult> ResultFailAsync<TEntity>(TEntity entity) => Ok(await ResultBase<TEntity>.FailAsync(entity));

        protected async Task<IActionResult> ExecuteAsync<TRequest, TResult>(IServiceBase<TRequest, TResult> serviceBase, TRequest request)
        {
            var executeCore = new ServiceCore<TRequest, TResult>(serviceBase);
            return Ok(serviceBase.ExecuteCore());
        }
    }

    public class Register
    {
        public void Registry(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISampleService, SampleService>();
        }
    }

    public interface ISampleService : IServiceBase<string, string>
    {
        
    }

    public class SampleService : ServiceBase<string, string>, ISampleService
    {
        public SampleService(ILogger logger, ISessionContext sessionContext) : base(logger, sessionContext)
        {
        }

        protected override (bool isContinue, string message) OnPreExecute(ISessionContext sessionContext, string request)
        {
            throw new NotImplementedException();
        }

        protected override string OnExecute(ISessionContext sessionContext, string Request)
        {
            throw new NotImplementedException();
        }

        protected override void OnPostExecute(ISessionContext sessionContext, string result)
        {
            throw new NotImplementedException();
        }
    }

    public class SampleDto
    {
        public string Name { get; set; }
        public class Validator : AbstractValidator<SampleDto>
        {
            public Validator()
            {
                RuleFor(m => m.Name).NotNull();
            }
        }
    }

    public interface ISample2Service : IServiceBase<SampleDto, string>
    {
    }

    [AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(ISample2Service))]
    [Transaction(TransactionScopeOption.Suppress)]
    public class Sample2Service : ServiceBase<SampleDto, string, SampleDto.Validator>
    {
        public Sample2Service(ILogger logger, ISessionContext sessionContext) : base(logger, sessionContext)
        {
        }

        protected override string OnExecute(ISessionContext sessionContext, SampleDto Request)
        {
            throw new NotImplementedException();
        }
    }
}