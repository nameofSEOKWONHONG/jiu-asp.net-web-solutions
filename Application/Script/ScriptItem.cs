namespace Application.Script;

/// <summary>
/// 스크립트 생성 기본
/// </summary>
internal record ScriptItem
{
    public string FileName { get; }
    public string Code { get; }
    public string Hash { get; }

    public ScriptItem(string fileName, string code, string hash)
    {
        this.FileName = fileName;
        this.Code = code;
        this.Hash = hash;
    }
}