namespace Application.Script;

/// <summary>
/// 스크립트 생성 기본
/// </summary>
internal record ScriptItem
{
    public string FileName { get; }
    public string Code { get; }
    public string HashSrcCode { get; }

    public ScriptItem(string fileName, string hashSrcCode, string code)
    {
        this.FileName = fileName;
        this.HashSrcCode = hashSrcCode;
        this.Code = code;
    }
}