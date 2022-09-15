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
        var dict = DictBuilder.TrainFromBuffer(new[] { buffer }, buffer.Length);
        using var options = new CompressionOptions(dict, compressionLevel: 22);
        using var compressor = new Compressor(options);
        var compressedData = compressor.Wrap(buffer);
        var filename = $"D://{Guid.NewGuid():N}.zstd";
        compressedData.xWriteFile(filename);
        
        var debuffer = filename.xFileReadAllBytes();
        var dedict = DictBuilder.TrainFromBuffer(new[] { debuffer }, debuffer.Length);
        using var deoptions = new DecompressionOptions(dedict);
        using var decompressor = new Decompressor(deoptions);
        var decompressedData = decompressor.Unwrap(debuffer);
        decompressedData.xWriteFile( $"D://{Guid.NewGuid():N}1.jpg");
    }
}