using System.Threading.Tasks;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Files;

public class LocalStorage : StorageProviderBase
{
    public LocalStorage(IOptionsSnapshot<StorageConfigOption> option):base(option)
    {
        
    }
    
    public override ENUM_STORAGE_TYPE GetStorageType() => ENUM_STORAGE_TYPE.LOCAL;
    
    public override async Task<IEnumerable<StorageResult>> UploadAsync(IEnumerable<IFormFile> files)
    {
        var result = new List<StorageResult>();
        var size = files.Sum(f => f.Length);
        foreach (var file in files)
        {
            var path = Path.GetTempFileName();
            using var stream = File.Create(path);
            await file.CopyToAsync(stream);
            result.Add(new StorageResult()
            {
                FileId = Guid.NewGuid().ToString("N"),
                FileUrl = null,
                Option = new StorageOption()
                {
                    BearerKey = "",
                    Etc = null,
                    Extension = file.FileName.xGetExtension(),
                    SrcFileName = file.FileName,
                    TempFileName = path,
                    Size = file.Length
                }
            });
        }

        return result;
    }

    
}