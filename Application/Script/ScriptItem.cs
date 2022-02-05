using eXtensionSharp;

namespace Application.Script;

/// <summary>
/// 스크립트 생성 기본
/// </summary>
internal record ScriptItem
{
    public string FileName { get; }
    public string Code { get; private set; }
    public string HashSrcCode { get; private set; }

    public ScriptItem(string fileName)
    {
        this.FileName = fileName;
        this.Code = this.FileName.xFileReadAllText();
        this.HashSrcCode = this.Code.xToHash();
    }
}