using System;

namespace Application.Infrastructure.Compression;

public interface ICompress
{
    Span<byte> Compress(string file);
    Span<byte> UnCompress(Span<byte> buffer);
}