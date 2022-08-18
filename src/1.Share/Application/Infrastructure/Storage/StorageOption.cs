using System.Collections.Generic;
using System.IO;

namespace Application.Infrastructure.Files;

public class StorageOption
{
    public MemoryStream Stream { get; set; }
    public string FileName { get; set; }
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
}