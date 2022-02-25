using Application.Abstract;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Abstract.Controllers;

/// <summary>
/// model base interface
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IModelBaseController<T, TRequest>
    where TRequest : class
{
    IServiceBase<T> Service { get; }
    
    Task<IActionResult> Get(TRequest request, int currentPage = 1, int pageSize = 50);
    
    Task<IActionResult> GetByKey(int key);

    Task<IActionResult> GetByGuid(Guid guid);

    Task<IActionResult> Create(TRequest request);

    Task<IActionResult> Update(TRequest request);

    Task<IActionResult> PatchByKey(int key, JsonPatchDocument<TRequest> patch);
    
    Task<IActionResult> PatchByGuid(Guid guid, JsonPatchDocument<TRequest> patch);    

    Task<IActionResult> DeleteByKey(int key);

    Task<IActionResult> DeleteByGuid(Guid guid);
}