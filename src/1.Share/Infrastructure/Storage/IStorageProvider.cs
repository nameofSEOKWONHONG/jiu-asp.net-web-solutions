using System.Threading.Tasks;
using Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Files;

/// <summary>
/// 업로드 공통 인터페이스
/// 각 구현별로 파일 정보 저장/조회/삭제 부분 구현해야 함.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// 파일 업로드
    /// 브라우저에서 업로드 직접하게 한다면? => cloud access 정보만 조회 > 브라우저에서 직접 업로드 > 이후 결과에 대한 처리
    /// 업로드시에 파일명 암호화 및 파일명 포함 파일 정보는 메타정보화 해야 함.
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    Task<IEnumerable<StorageResult>> UploadAsync(IEnumerable<IFormFile> file);

    ENUM_STORAGE_TYPE GetStorageType();
}

/// <summary>
/// 업로드 구현 추상 클래스
/// Option설정값에 따라 연결 대상 Cloud 설정 구현해야 함.
/// </summary>
public abstract class StorageProviderBase : IStorageProvider
{
    private readonly StorageConfigOption _option;
    public StorageProviderBase(IOptionsSnapshot<StorageConfigOption> option)
    {
        _option = option.Value;
    }
    public abstract Task<IEnumerable<StorageResult>> UploadAsync(IEnumerable<IFormFile> file);
    public abstract ENUM_STORAGE_TYPE GetStorageType();
}
