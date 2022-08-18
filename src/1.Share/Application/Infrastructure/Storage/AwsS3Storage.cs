using System;
using System.Threading.Tasks;

namespace Application.Infrastructure.Files;

public class AwsS3Storage : StorageProviderBase
{
    public override Task<StorageResult> UploadAsync(StorageOption option)
    {
        throw new NotImplementedException();
    }
}