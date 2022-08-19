using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Storage;
using eXtensionSharp;
using Infrastructure;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Infrastructure.Storage.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Utils;

namespace WebApiApplication.Controllers;

public class StorageController : SessionController<StorageController>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageProvider _storageProvider;
    public StorageController(ApplicationDbContext context,
        StorageProviderResolver storageProviderResolver)
    {
        _dbContext = context;
        _storageProvider = storageProviderResolver(ENUM_STORAGE_TYPE.LOCAL);
    }

    [HttpGet]
    public async Task<IActionResult> GetFileInfo(string Id)
    {
        var result = _dbContext.Storages.FirstOrDefault(m => m.ID == Guid.Parse(Id));
        return await ResultOkAsync(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        var result = await _storageProvider.UploadAsync(files);
        foreach (var storageResult in result)
        {
            await _dbContext.Storages.AddAsync(new TB_STORAGE()
            {
                CLOUD_TYPE = _storageProvider.GetStorageType().xValue<short>(),
                FILE_NAME = storageResult.Option.SrcFileName,
                FILE_SIZE = storageResult.Option.Size,
                FILE_URL = null,
                OWNER_KEY = string.Empty,
                WRITE_ID = SessionContext.UserId.ToString(),
                WRITE_DT = DateTime.Now,
                UPDATE_ID = SessionContext.UserId.ToString(),
                UPDATE_DT = DateTime.Now
            });
        }
        await _dbContext.SaveChangesAsync();
        return await ResultOkAsync(result);
    }
}