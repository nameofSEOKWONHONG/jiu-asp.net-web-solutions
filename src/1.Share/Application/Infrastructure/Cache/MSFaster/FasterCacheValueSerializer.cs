using FASTER.core;

namespace Application.Infrastructure.Cache.MSFaster;

/// <summary>
/// Serializer for CacheValue - used if CacheValue is changed from struct to class
/// </summary>
public class FasterCacheValueSerializer : BinaryObjectSerializer<FasterCacheValue>
{
    public override void Deserialize(out FasterCacheValue obj)
    {
        obj = new FasterCacheValue(reader.ReadString());
    }

    public override void Serialize(ref FasterCacheValue obj)
    {
        writer.Write(obj.value);
    }
}