using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Base;
using Application.Context;
using CSScripting;
using Domain.Entities;
using Domain.Request;
using Domain.Response;
using eXtensionSharp;
using Infrastructure.Abstract.Controllers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApiApplication.Controllers;

public class ModelVersionSampleController : ModelVersionController<SampleDto, RequestBase<SampleDto>, ResultBase<SampleDto>>
{
    public ModelVersionSampleController(ISampleRepository repository) : base(repository)
    {
        
    }
    
    public override Task<IActionResult> Get(RequestBase<SampleDto> request, int currentPage = 1, int pageSize = 50)
    {
        var result = Repository.Query(request.Data, currentPage, pageSize);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> GetByKey(int key)
    {
        var result = Repository.Fetch(new SampleDto() {Id = key});
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> GetByGuid(Guid guid)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> Create(RequestBase<SampleDto> request)
    {
        var result = Repository.Create(request.Data);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> Update(RequestBase<SampleDto> request)
    {
        var result = Repository.Update(request.Data);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> PatchByKey(int key, JsonPatchDocument<RequestBase<SampleDto>> patch)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> PatchByGuid(Guid guid, JsonPatchDocument<RequestBase<SampleDto>> patch)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> DeleteByKey(int key)
    {
        var result = Repository.Delete(new SampleDto() {Id = key});
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> DeleteByGuid(Guid guid)
    {
        throw new NotImplementedException();
    }
}

public interface ISampleRepository : IRepositoryBase<SampleDto>
{
    
}

public class SampleRepository : RepositoryBase<ApplicationDbContext, TB_USER>
{
    public SampleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override TB_USER Fetch(TB_USER item)
    {
        return _dbContext.Users.First(m => m.ID == item.ID);
    }

    public override IEnumerable<TB_USER> Query(TB_USER requst, int currentPage = 1, int pageSize = 50)
    {
        return _dbContext.Users.Where(m => m.EMAIL.Contains(requst.EMAIL))
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}