namespace ImageResize;

public class RxtFileReader
{
    private readonly FileInfo _rxtFileInfo;
    public RxtFileReader(string rxtFileName)
    {
        if (rxtFileName is null) throw new Exception("rxt file not found");
        _rxtFileInfo = new FileInfo(rxtFileName);
    }

    public IEnumerable<RxtSet> Read()
    {
        var result = new List<RxtSet>();
        var lines = File.ReadLines(_rxtFileInfo.FullName);
        foreach (var line in lines)
        {
            var args = line.Split(" ");
            result.Add(new RxtSet()
            {
                FileName = args[0],
                Size = int.Parse(args[1]),
                Quality = int.Parse(args[2])
            });
        }

        return result;
    }
}

public class RxtSet
{
    public string FileName { get; set; }
    public int Size { get; set; }
    public int Quality { get; set; }
}