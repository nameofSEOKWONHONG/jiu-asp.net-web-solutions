using Application.Base;
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
    EntityVersionController<T, TRequest, TResponse> : VersionController<EntityVersionController<T, TRequest, TResponse>>, IEntityBaseController<T, TRequest>
    where TRequest : RequestBase<T>
    where TResponse : ResultBase<T>
{
    public IRepositoryBase<T> Repository { get; }

    protected EntityVersionController(IRepositoryBase<T> repository)
    {
        this.Repository = repository;
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