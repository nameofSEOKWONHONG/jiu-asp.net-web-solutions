using System.IO.Compression;
using eXtensionSharp;
using IronPython.Runtime;

namespace MailSendWorkerService;

public class MailFormReader
{
    private readonly ILogger _logger;
    private readonly string _mailFolderPath = "mails".xCurrentPath();
    private readonly string _mailContentsFileName = "Contents.txt";
    public MailFormReader(ILogger<MailFormReader> logger)
    {
        _logger = logger;
    }

    public IEnumerable<MailFileDto> GetMailFiles()
    {
        var mailFiles = new List<MailFileDto>();
        var files = Directory.GetFiles(_mailFolderPath).Take(100);
        foreach (var file in files)
        {
            var mailForm = new MailFormDto();
            mailFiles.Add(new MailFileDto(file, mailForm));
            
            if(file.xFileExists()) continue;
            var extractPath = file.xGetFileNameWithoutExtension();
            ZipFile.CreateFromDirectory(_mailFolderPath, file);
            ZipFile.ExtractToDirectory(file, extractPath);
            var lines = File.ReadAllLines(Path.Combine(extractPath, _mailContentsFileName));
            mailForm.Sender = lines[0];
            mailForm.Receivers = lines[1].xSplit().ToArray();
            mailForm.Title = lines[2];
            mailForm.Contents = lines[3];
            mailForm.Files = lines[4].xSplit().ToArray();
        }

        return mailFiles;
    }

    public void RemoveMailFiles(IEnumerable<MailFileDto> mailFiles)
    {
        foreach (var mailFile in mailFiles)
        {
            var filePath = mailFile.ZipFileName.xGetFileNameWithoutExtension();
            var removePath = Path.Combine(_mailFolderPath, filePath);
            if (Directory.Exists(removePath))
            {
                Directory.Delete(removePath, true);
            }
            File.Delete(mailFile.ZipFileName);
        }
    }
}

public record MailFileDto(string ZipFileName, MailFormDto MailFormItem);
public class MailFormDto
{
    public string Sender { get; set; }
    public string[] Receivers { get; set; }
    public string Title { get; set; }
    public string Contents { get; set; }
    public string[] Files { get; set; }
}