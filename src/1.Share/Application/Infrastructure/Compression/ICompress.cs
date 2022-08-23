using System;

namespace Application.Infrastructure.Compression;

public interface ICompress
{
    byte[] Compress(string file);
    byte[] Compress(byte[] bytes);
    byte[] UnCompress(Span<byte> buffer);
}