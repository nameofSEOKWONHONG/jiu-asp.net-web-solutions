using System.Threading.Tasks;

namespace Application.Infrastructure.Files;

public abstract class StorageProviderBase : IStorageProvider
{
    public abstract Task<StorageResult> UploadAsync(StorageOption option);
}

public interface IStorageProvider
{
    Task<StorageResult> UploadAsync(StorageOption option);
}
