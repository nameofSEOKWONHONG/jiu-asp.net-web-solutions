using System.Threading.Tasks;

namespace Application.Infrastructure.Files;

public interface IStorageProvider
{
    /// <summary>
    /// 파일 업로드
    /// 브라우저에서 업로드 직접하게 한다면? => cloud access 정보만 조회 > 브라우저에서 직접 업로드 > 이후 결과에 대한 처리  
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    Task<StorageResult> UploadAsync(StorageOption option);
}

public abstract class StorageProviderBase : IStorageProvider
{
    public abstract Task<StorageResult> UploadAsync(StorageOption option);
}
