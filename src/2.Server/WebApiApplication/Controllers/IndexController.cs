using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Context;
using Application.Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using eXtensionSharp;
using FluentValidation;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Controllers
{
    [AllowAnonymous]
    public class IndexController : ApiControllerBase<IndexController>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationDbContext _dbContext;
        
        public IndexController(CacheProviderResolver resolver, 
            ApplicationDbContext dbContext)
        {
            _cacheProvider = resolver(ENUM_CACHE_TYPE.FASTER);
            _dbContext = dbContext;
        }

        [HttpGet("CacheSample")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> CacheSample()
        {
            var key = "CacheSample";
            var result = _cacheProvider.GetCache<string>(key);
            if (result.xIsEmpty())
            {
                result = await Task.Factory.StartNew(() =>
                {
                    var sb = new XStringBuilder();
                    sb.AppendLine("Hello, The purpose of this project is to create a template similar to \"typescript-express-starter\"");
                    sb.AppendLine("(https://github.com/ljlm0402/typescript-express-starter)");
                    sb.AppendLine("git clone [project site] [your project name]");
                    sb.AppendLine("cd [your project name]");
                    sb.AppendLine("dotnet restore");
                    sb.AppendLine("dotnet run or dotnet watch run");
                    sb.Release(out string str);
                    return str;
                });
                _cacheProvider.SetCache<string>(key, result);
            }
            
            return Ok(result);
        }

        [HttpGet("xThenAsyncSample")]
        public async Task<IActionResult> xThenAsyncSample()
        {
            var result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                var response = await client.GetAsync("api/index/JavascriptSample?api-version=1");
                await response.xThenAsync(async self =>
                {
                    self.EnsureSuccessStatusCode();
                    result = await self.Content.ReadAsStringAsync();
                }, e =>
                {
                    _logger.LogInformation(e, e.Message);
                    throw e;
                });
            }

            return Ok(result);
        }
        
        [HttpPost("sample")]
        public IActionResult Sample(SampleDto dto)
        {
            var (isValid, result) = this.TryValidate<SampleDto, SampleDto.Validator>(dto);
            return isValid.xIsFalse() ? this.ResultOk(result) : this.ResultOk(dto);
        } 
        
        /// <summary>
        /// not working
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpPost("sample2")]
        [Obsolete("don't use case")]
        public IActionResult Sample2([FromBody]ENUM_ROLE_TYPE str)
        {
            return Ok(str);
        }


    }

    public class SampleDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public ENUM_ROLE_TYPE RoleType { get; set; }
        [Required]
        [MinLength(1), MaxLength(5)]
        public string Name { get; set; }
        
        public class Validator : AbstractValidator<SampleDto>
        {
            public Validator()
            {
                RuleFor(m => m.Id).NotEmpty();
                RuleFor(m => m.Id).GreaterThanOrEqualTo(1);
                RuleFor(m => m.RoleType).Equal(ENUM_ROLE_TYPE.USER);
                RuleFor(m => m.Name).Equal("test");
            }
        }
    }
}