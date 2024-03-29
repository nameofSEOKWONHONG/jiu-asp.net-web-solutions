﻿using System.Transactions;
using Application.Base;
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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected bool TryValidate<T>(T entity, out ActionResult result) 
            where T : class
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
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <returns></returns>
        protected (bool IsValid, DynamicDictionary<List<string>> result) TryValidate<T, TValidator>(T data)
            where T : class
            where TValidator : AbstractValidator<T>, new()
        {
            return ValidatorCore.TryValidate<T, TValidator>(data);
        }

        protected async Task<(bool IsValid, DynamicDictionary<List<string>> result)> TryValidateAsync<T, TValidator>(T data)
            where T : class
            where TValidator : AbstractValidator<T>, new()
        {
            return await ValidatorCore.TryValidateAsync<T, TValidator>(data);
        }

        protected IActionResult ResultOk<T>(T data) => Ok(ResultBase<T>.Success(data));
        
        protected IActionResult ResultOk(string message) => Ok(ResultBase.Success(message));

        protected IActionResult ResultFail<T>(T data) => Ok(ResultBase<T>.Fail(data));
        
        protected IActionResult ResultFail(string message) => Ok(ResultBase.Fail(message));
        
        protected async Task<IActionResult> ResultOkAsync<T>(T data) => Ok(await ResultBase<T>.SuccessAsync(data));
        
        protected async Task<IActionResult> ResultOkAsync(string message) => Ok(await ResultBase.SuccessAsync(message));

        protected async Task<IActionResult> ResultFailAsync<T>(T data) => Ok(await ResultBase<T>.FailAsync(data));
        
        protected async Task<IActionResult> ResultFailAsync(string message) => Ok(await ResultBase.FailAsync(message));

        protected async Task<IActionResult> ExecuteAsync<TRequest, TResult>(IServiceBase<TRequest, TResult> serviceBase, TRequest request)
        {
            var executeCore = new ServiceCore<TRequest, TResult>(serviceBase);
            serviceBase.ExecuteCore();
            return Ok(serviceBase.Result);
        }
    }
}