using Application.Abstract;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Abstract.Controllers;

/// <summary>
/// data controller by session base
/// </summary>
/// <typeparam name="T">Entity, Dto</typeparam>
/// <typeparam name="TRequest">RequestBase<T></typeparam>
/// <typeparam name="TResponse">ResultBase</typeparam>
public abstract class ModelSessionController<T, TRequest, TResponse> : SessionController<ModelSessionController<T, TRequest, TResponse>>, IModelBaseController<T, TRequest>
    where TRequest : class
    where TResponse : class
{
    public IServiceBase<T> Service { get; }
    
    protected ModelSessionController(IServiceBase<T> service)
    {
        this.Service = service;
    }
    
    public abstract Task<IActionResult> Get(TRequest request, int currentPage = 1, int pageSize = 50);

    public abstract Task<IActionResult> GetByKey(int key);

    public abstract Task<IActionResult> GetByGuid(Guid guid);

    public abstract Task<IActionResult> Create(TRequest request);

    public abstract Task<IActionResult> Update(TRequest request);

    public abstract Task<IActionResult> PatchByKey(int key, JsonPatchDocument<TRequest> patch);

    public abstract Task<IActionResult> PatchByGuid(Guid guid, JsonPatchDocument<TRequest> patch);

    public abstract Task<IActionResult> DeleteByKey(int key);

    public abstract Task<IActionResult> DeleteByGuid(Guid guid);
}