namespace Application.Script;

internal record ScriptorItem
{
    public string FileName { get; }
    public string Code { get; }
    public string Hash { get; }

    public ScriptorItem(string fileName, string code, string hash)
    {
        this.FileName = fileName;
        this.Code = code;
        this.Hash = hash;
    }
}