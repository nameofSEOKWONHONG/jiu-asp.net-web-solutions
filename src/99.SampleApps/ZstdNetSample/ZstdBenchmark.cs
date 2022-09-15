using BenchmarkDotNet.Attributes;
using eXtensionSharp;
using ZstdNet;

namespace ZstdNetSample;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ZstdBenchmark
{
    [Benchmark()]
    public void Runner()
    {
        var buffer = "D://Snake_River_(5mb).jpg".xFileReadAllBytes();
        using var options = new CompressionOptions(buffer, compressionLevel: 18);
        using var compressor = new Compressor(options);
        var compressedData = compressor.Wrap(buffer);
        compressedData.xWriteFile($"D://{Guid.NewGuid():N}.zip");
    }
}