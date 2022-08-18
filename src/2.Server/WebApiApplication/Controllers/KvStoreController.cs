using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Application.Infrastructure.Files;
using Domain.Entities.KeyValueStore;
using Elsa.Models;
using eXtensionSharp;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetBox.Extensions;
using Serilog;

namespace WebApiApplication.Controllers;

public class KvStoreController : ApiControllerBase<KvStoreController>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageProvider _storageProvider;
    public KvStoreController(ApplicationDbContext dbContext,
        StorageProviderResolver storageProviderResolver)
    {
        _dbContext = dbContext;
        _storageProvider = storageProviderResolver.Invoke(ENUM_STORAGE_TYPE.NCP);
    }

    [HttpGet("GetKvById")]
    public async Task<IActionResult> GetKvById(int id, string key)
    {
        var exists = _dbContext.KvStores.xFirst(m => m.ID == id && m.KEY == key);
        if (exists.xIsEmpty()) return await ResultFailAsync("Not Found");
        return await ResultOkAsync(exists);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetKvAll()
    {
        //쿼리 실행 안됨.
        var items = _dbContext.KvStores.ToAsyncEnumerable();
        
        //쿼리 실행됨.
        await foreach (var item in items)
        {
            item.KV = $"{item.KEY}^{item.VAL}";
        }
        //메모리에 적재된 내역 반환.
        return await ResultOkAsync(items.ToEnumerable());
    }

    [HttpPost]
    public async Task<IActionResult> AddKv(TB_KV_STORE item, IFormFile file)
    {
        var result = await _storageProvider.UploadAsync(new StorageOption()
        {
            Extension = file.FileName.xGetFileExtension(),
            FileName = file.FileName,
            Stream = new MemoryStream(file.OpenReadStream().ToByteArray())
        });

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