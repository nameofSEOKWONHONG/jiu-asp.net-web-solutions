﻿using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ApiControllerBase<T> : ControllerBase
    {
        private ILogger<T> _loggerInstance;
        private IMediator _mediatorInstance;
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
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