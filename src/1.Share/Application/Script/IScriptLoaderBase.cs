namespace Application.Script;

public interface IScriptLoaderBase
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
