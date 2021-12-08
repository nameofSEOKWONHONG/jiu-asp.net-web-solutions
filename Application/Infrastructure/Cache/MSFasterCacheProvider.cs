using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using eXtensionSharp;
using FASTER.core;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure.Cache
{
    /// <summary>
    /// TODO : 나머지 구현해야 함.
    /// 로컬 메모리/스토리지 캐시 적용 케이스임.
    /// 전용 TCP 서버를 구축하려는 경우 다음 URL 참조
    /// https://microsoft.github.io/FASTER/docs/remote-basics/
    /// </summary>
    public class MSFasterCacheProvider : CacheProviderBase, ICacheProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FasterKV<CacheKey, CacheValue> _fasterKV;
        private readonly bool useReadCache = true;
        public MSFasterCacheProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _fasterKV = InitFasterKV();
        }

        // Whether we enable a read cache
        
        
        private FasterKV<CacheKey, CacheValue> InitFasterKV()
        {
            // Create files for storing data
            var path = Path.GetTempPath() + "FasterCache/";
            var log =  Devices.CreateLogDevice(path + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings {
                LogDevice = log, 
                ObjectLogDevice = objlog,
                ReadCacheSettings = useReadCache ? new ReadCacheSettings() : null,
                // Uncomment below for low memory footprint demo
                // PageSizeBits = 12, // (4K pages)
                // MemorySizeBits = 20 // (1M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<CacheKey, CacheValue> {
                keySerializer = () => new CacheKeySerializer(),
                valueSerializer = () => new CacheValueSerializer()
            };

            // Create instance of store
            return new FasterKV<CacheKey, CacheValue>(
                size: 1L << 20,
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointDir = path },
                serializerSettings: serializerSettings,
                comparer: new CacheKey(string.Empty)
            );            
        }

        public int Count { get; }
        public T GetCache<T>()
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(string key)
        {
            using var store = _fasterKV.For(new CacheFunctions()).NewSession<CacheFunctions>();
            var k = new CacheKey(this.CreateCacheKey(key));
            var output = default(CacheValue);
            store.Read(ref k, ref output);
            if (output.value.xIsEmpty()) return default;
            return output.value.xToEntity<T>();
        }

        public void SetCache<T>(T value, int? expireTimeout)
        {
            using var store = _fasterKV.For(new CacheFunctions()).NewSession<CacheFunctions>();
            var k = new CacheKey(this.CreateCacheKey());
            var v = new CacheValue(value.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void SetCache<T>(string key, T value, int? expireTimeout)
        {
            using var store = _fasterKV.For(new CacheFunctions()).NewSession<CacheFunctions>();
            var k = new CacheKey(this.CreateCacheKey(key));
            var v = new CacheValue(value.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? duration)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache<T>(CacheOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
    
    #region [cache key/value class]
    public struct CacheKey : IFasterEqualityComparer<CacheKey>
    {
        public string key;

        public CacheKey(string first)
        {
            key = first;
        }

        public long GetHashCode64(ref CacheKey k)
        {
            return k.GetHashCode();
        }

        public bool Equals(ref CacheKey k1, ref CacheKey k2)
        {
            return k1.key == k2.key;
        }
    }

    public struct CacheValue
    {
        public string value;

        public CacheValue(string first)
        {
            value = first;
        }
    }

    /// <summary>
    /// Serializer for CacheKey - used if CacheKey is changed from struct to class
    /// </summary>
    public class CacheKeySerializer : BinaryObjectSerializer<CacheKey>
    {
        public override void Deserialize(out CacheKey obj)
        {
            obj = new CacheKey(reader.ReadString());
        }

        public override void Serialize(ref CacheKey obj)
        {
            writer.Write(obj.key);
        }
    }

    /// <summary>
    /// Serializer for CacheValue - used if CacheValue is changed from struct to class
    /// </summary>
    public class CacheValueSerializer : BinaryObjectSerializer<CacheValue>
    {
        public override void Deserialize(out CacheValue obj)
        {
            obj = new CacheValue(reader.ReadString());
        }

        public override void Serialize(ref CacheValue obj)
        {
            writer.Write(obj.value);
        }
    }

    /// <summary>
    /// User context to measure latency and/or check read result
    /// </summary>
    public struct CacheContext
    {
        public int type;
        public long ticks;
    }

    /// <summary>
    /// Callback for FASTER operations
    /// </summary>
    public class CacheFunctions : SimpleFunctions<CacheKey, CacheValue, CacheContext>
    {
        public override void ReadCompletionCallback(ref CacheKey key, ref CacheValue input, ref CacheValue output, CacheContext ctx, Status status)
        {
            if (ctx.type == 0)
            {
                if (output.value != key.key)
                    throw new Exception("Read error!");
            }
            else
            {
                long ticks = Stopwatch.GetTimestamp() - ctx.ticks;

                if (status == Status.NOTFOUND)
                    Console.WriteLine("Async: Value not found, latency = {0}ms", 1000 * (ticks - ctx.ticks) / (double)Stopwatch.Frequency);

                if (output.value != key.key)
                    Console.WriteLine("Async: Incorrect value {0} found, latency = {1}ms", output.value, new TimeSpan(ticks).TotalMilliseconds);
                else
                    Console.WriteLine("Async: Correct value {0} found, latency = {1}ms", output.value, new TimeSpan(ticks).TotalMilliseconds);
            }
        }
    }
    #endregion
    
}