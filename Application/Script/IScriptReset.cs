namespace Application.Script;

/// <summary>
/// 스크립트 리셋을 위한 interface
/// </summary>
public interface IScriptReset
{
    /// <summary>
    /// 현재 버전
    /// </summary>
    double Version { get; set; }
    /// <summary>
    /// 리셋
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    bool Reset(string fileName = null);
}