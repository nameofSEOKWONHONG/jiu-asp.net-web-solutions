namespace Application.Script.CsScript;

internal class SharpScriptItem
{
    public string FileName { get; }
    public string Code { get; }
    public string Hash { get; }

    public SharpScriptItem(string fileName, string code, string hash)
    {
        this.FileName = fileName;
        this.Code = code;
        this.Hash = hash;
    }
}