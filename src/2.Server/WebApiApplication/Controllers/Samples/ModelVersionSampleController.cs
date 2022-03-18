using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstract;
using Domain.Request;
using Domain.Response;
using eXtensionSharp;
using Infrastructure.Abstract.Controllers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace WebApiApplication.Controllers;

public class ModelVersionSampleController : ModelVersionController<SampleDto, RequestBase<SampleDto>, ResultBase<SampleDto>>
{
    public ModelVersionSampleController(ISampleService service) : base(service)
    {
        
    }
    
    public override Task<IActionResult> Get(RequestBase<SampleDto> request, int currentPage = 1, int pageSize = 50)
    {
        var result = Service.SelectAll(request.Data, currentPage, pageSize);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> GetByKey(int key)
    {
        var result = Service.Select(new SampleDto() {Id = key});
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> GetByGuid(Guid guid)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> Create(RequestBase<SampleDto> request)
    {
        var result = Service.Create(request.Data);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> Update(RequestBase<SampleDto> request)
    {
        var result = Service.Update(request.Data);
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
        var result = Service.Delete(new SampleDto() {Id = key});
        return Task.FromResult<IActionResult>(Ok(result));
    }

    public override Task<IActionResult> DeleteByGuid(Guid guid)
    {
        throw new NotImplementedException();
    }
}

public interface ISampleService : IServiceBase<SampleDto>
{
    
}

public class SampleService : ISampleService
{
    public SampleService()
    {
    }
    
    private readonly List<SampleDto> _items = new List<SampleDto>();
    
    public ResultBase<SampleDto> Create(SampleDto item)
    {
        _items.Add(item);
        return ResultBase<SampleDto>.Success(item);
    }

    public ResultBase<IEnumerable<SampleDto>> CreateBulk(IEnumerable<SampleDto> items)
    {
        throw new NotImplementedException();
    }

    public ResultBase<SampleDto> Update(SampleDto item)
    {
        var exists = _items.FirstOrDefault(m => m.Id == item.Id);
        return exists.xIfNotEmpty<SampleDto, ResultBase<SampleDto>>(() =>
        {
            exists.Name = item.Name;
            exists.RoleType = item.RoleType;
            return ResultBase<SampleDto>.Success(exists);
        }, () => ResultBase<SampleDto>.Fail(exists));
    }

    public ResultBase<IEnumerable<SampleDto>> UpdateBulk(IEnumerable<SampleDto> items)
    {
        throw new NotImplementedException();
    }

    public ResultBase<SampleDto> Delete(SampleDto item)
    {
        var exists = _items.FirstOrDefault(m => m.Id == item.Id);
        var result = exists.xIfNotEmpty<SampleDto, ResultBase<SampleDto>>(
            () => _items.Remove(exists) ? ResultBase<SampleDto>.Success(exists) : ResultBase<SampleDto>.Fail(), ResultBase<SampleDto>.Fail);

        return result;
    }

    public ResultBase<IEnumerable<SampleDto>> DeleteBulk(IEnumerable<SampleDto> items)
    {
        throw new NotImplementedException();
    }

    public ResultBase<SampleDto> Select(SampleDto item)
    {
        var result = _items.FirstOrDefault(m => m.Id == item.Id);
        return ResultBase<SampleDto>.Success(result);

    }

    public ResultBase<IEnumerable<SampleDto>> SelectAll(SampleDto request, int currentPage = 1, int pageSize = 50)
    {
        var result = _items.Where(m => m.Name.Contains(request.Name));
        return ResultBase<IEnumerable<SampleDto>>.Success(result);
    }
}