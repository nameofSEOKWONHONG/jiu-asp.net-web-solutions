using FASTER.core;

namespace Application.Infrastructure.Cache.MSFaster;

/// <summary>
/// Serializer for CacheKey - used if CacheKey is changed from struct to class
/// </summary>
public class FasterCacheKeySerializer : BinaryObjectSerializer<FasterCacheKey>
{
    public override void Deserialize(out FasterCacheKey obj)
    {
        obj = new FasterCacheKey(reader.ReadString());
    }

    public override void Serialize(ref FasterCacheKey obj)
    {
        writer.Write(obj.key);
    }
}