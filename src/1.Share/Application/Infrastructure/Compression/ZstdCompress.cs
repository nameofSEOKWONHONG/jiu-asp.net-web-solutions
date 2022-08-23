using System;
using System.Collections.Generic;
using System.IO;
using eXtensionSharp;
using ZstdSharp;

namespace Application.Infrastructure.Compression;

/// <summary>
/// ref : https://github.com/oleg-st/ZstdSharp
/// ref : https://github.com/facebook/zstd
/// zstd 대용량의 경우 30%까지 용량 절약, 백업용으로 사용하면 좋을 듯 함.
/// TODO : zip으로 아카이브 생성시 Action으로 설정할 수 있도록 할 예정 
/// </summary>
public class ZstdCompress : ICompress
{
    public byte[] Compress(string file)
    {
        var fastest = -7;
        var ultra = 22;
        var src = file.xFileReadAllBytes();
        using var compressor = new Compressor(ultra);
        return compressor.Wrap(src).ToArray();
    }

    public byte[] Compress(byte[] bytes)
    {
        var fastest = -7;
        var ultra = 22;
        using var compressor = new Compressor(ultra);
        return compressor.Wrap(bytes).ToArray();
    }

    public byte[] UnCompress(Span<byte> buffer)
    {
        var fastest = -7;
        var ultra = 22;
        using var decompressor = new Decompressor();
        return decompressor.Unwrap(buffer).ToArray();
    }
}