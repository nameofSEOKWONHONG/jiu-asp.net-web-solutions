using System.Collections.Generic;
using System.IO;

namespace Infrastructure.Storage.Files;

public class StorageOption
{
    public string TempFileName { get; set; }
    public string SrcFileName { get; set; }
    public long Size { get; set; }
    public string Extension { get; set; }
    public string BearerKey { get; set; }
    public Dictionary<string, string> Etc { get; set; }
}

public class StorageCommitOption
{
    public string FileId { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
}

public class StorageResult
{
    public string FileId { get; set; }
    public string FileUrl { get; set; }
    public StorageOption Option { get; set; }
}