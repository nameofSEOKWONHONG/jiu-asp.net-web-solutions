using Application.Abstract;
using Domain.Request;
using Domain.Response;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Abstract.Controllers;

/// <summary>
/// data Controller by version base
/// </summary>
/// <typeparam name="T">Entity, Dto</typeparam>
/// <typeparam name="TRequest">RequestBase<T></typeparam>
/// <typeparam name="TResponse">ResultBase</typeparam>
public abstract class
    ModelVersionController<T, TRequest, TResponse> : VersionController<ModelVersionController<T, TRequest, TResponse>>, IModelBaseController<T, TRequest>
    where TRequest : RequestBase<T>
    where TResponse : ResultBase<T>
{
    public IServiceBase<T> Service { get; }

    protected ModelVersionController(IServiceBase<T> service)
    {
        this.Service = service;
    }

    [HttpPost("Get")]
    public abstract Task<IActionResult> Get(TRequest request, int currentPage = 1, int pageSize = 50);
    
    [HttpGet("GetByKey/{key}")]
    public abstract Task<IActionResult> GetByKey(int key);

    [HttpGet("GetByGuid/{guid}")]
    public abstract Task<IActionResult> GetByGuid(Guid guid);

    [HttpPost("Create")]
    public abstract Task<IActionResult> Create(TRequest request);

    [HttpPut("Update")]
    public abstract Task<IActionResult> Update(TRequest request);

    [HttpPatch("PatchByKey")]
    public abstract Task<IActionResult> PatchByKey(int key, JsonPatchDocument<TRequest> patch);
    
    [HttpPatch("PatchByGuid")]
    public abstract Task<IActionResult> PatchByGuid(Guid guid, JsonPatchDocument<TRequest> patch);    

    [HttpDelete("DeleteByKey")]
    public abstract Task<IActionResult> DeleteByKey(int key);

    [HttpDelete("DeleteByGuid")]
    public abstract Task<IActionResult> DeleteByGuid(Guid guid);
}