using System.Threading.Tasks;
using Domain.Entities.KeyValueStore;
using Elsa.Models;
using eXtensionSharp;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace WebApiApplication.Controllers;

public class KvStoreController : ApiControllerBase<KvStoreController>
{
    private readonly ApplicationDbContext _dbContext;
    public KvStoreController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetKvById(int id, string key)
    {
        var exists = _dbContext.KvStores.xFirst(m => m.ID == id && m.KEY == key);
        if (exists.xIsEmpty()) return await ResultFailAsync("Not Found");
        return await ResultOkAsync(exists);
    }

    [HttpPost]
    public async Task<IActionResult> AddKv(TB_KV_STORE item)
    {
        var exists = _dbContext.KvStores.xFirst(m => m.ID == item.ID && m.KEY == item.KEY);
        if (exists.xIsNotEmpty()) return await ResultFailAsync<TB_KV_STORE>(item);
        _dbContext.KvStores.Add(item);
        await _dbContext.SaveChangesAsync();
        return await ResultOkAsync(item);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateKv(TB_KV_STORE item)
    {
        var exists = _dbContext.KvStores.xFirst(m => m.ID == item.ID && m.KEY == item.KEY);
        if (exists.xIsEmpty()) return await ResultFailAsync("Not Found");
        exists.VAL = item.VAL;
        _dbContext.KvStores.Update(exists);
        await _dbContext.SaveChangesAsync();
        return await ResultOkAsync(exists);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(int id)
    {
        var exists = _dbContext.KvStores.xFirst(m => m.ID == id);
        if (exists.xIsEmpty()) return await ResultFailAsync("Not Found");
        _dbContext.KvStores.Remove(exists);
        await _dbContext.SaveChangesAsync();
        return await ResultOkAsync(exists);
    }
    
    //HttpPatch는 생략
}