using System;
using System.Threading.Tasks;

namespace Application.Infrastructure.Files;

public class AzureStorage : StorageProviderBase
{
    public override Task<StorageResult> UploadAsync(StorageOption option)
    {
        throw new NotImplementedException();
    }
}