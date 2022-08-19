using System;
using System.Threading.Tasks;
using Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Files;

public class AwsS3Storage : StorageProviderBase
{
    public AwsS3Storage(IOptionsSnapshot<StorageConfigOption> option):base(option)
    {
        
    }
    
    public override Task<IEnumerable<StorageResult>> UploadAsync(IEnumerable<IFormFile> file)
    {
        throw new NotImplementedException();
    }

    public override ENUM_STORAGE_TYPE GetStorageType() => ENUM_STORAGE_TYPE.AWS_S3;
}