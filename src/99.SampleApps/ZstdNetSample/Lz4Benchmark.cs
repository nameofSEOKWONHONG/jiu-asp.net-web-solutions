using BenchmarkDotNet.Attributes;
using K4os.Compression.LZ4.Streams;

namespace ZstdNetSample;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Lz4Benchmark
{
    [Benchmark()]
    public void Runner()
    {
        var srcfileName = "D://Snake_River_(5mb).jpg";
        var targetfileName = $"{Guid.NewGuid():N}" + ".lz4";
        using (var source = File.OpenRead(srcfileName))
        using (var target = LZ4Stream.Encode(File.Create(targetfileName)))
        {
            source.CopyTo(target);
        }
        
        using (var source = LZ4Stream.Decode(File.OpenRead(targetfileName)))
        using (var target = File.Create($"{Guid.NewGuid():N}1.jpg"))
        {
            source.CopyTo(target);
        }
    }
}