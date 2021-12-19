using System;
using System.IO;
using eXtensionSharp;
using FASTER.core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Infrastructure.Cache.MSFaster
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
        private readonly ILogger _logger;
        private readonly FasterKV<FasterCacheKey, FasterCacheValue> _fasterKV;
        private readonly bool useReadCache = true;

        public MSFasterCacheProvider(IHttpContextAccessor httpContextAccessor, ILogger<MSFasterCacheProvider> logger) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fasterKV = InitFasterKV();
        }

        // Whether we enable a read cache


        private FasterKV<FasterCacheKey, FasterCacheValue> InitFasterKV()
        {
            // Create files for storing data
            var path = Path.GetTempPath() + "FasterCache/";
            var log = Devices.CreateLogDevice(path + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = useReadCache ? new ReadCacheSettings() : null,
                // Uncomment below for low memory footprint demo
                // PageSizeBits = 12, // (4K pages)
                // MemorySizeBits = 20 // (1M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<FasterCacheKey, FasterCacheValue>
            {
                keySerializer = () => new FasterCacheKeySerializer(),
                valueSerializer = () => new FasterCacheValueSerializer()
            };

            // Create instance of store
            return new FasterKV<FasterCacheKey, FasterCacheValue>(
                size: 1L << 20,
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointDir = path },
                serializerSettings: serializerSettings,
                comparer: new FasterCacheKey(string.Empty)
            );
        }

        public int Count { get; }

        public T GetCache<T>()
        {
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey(this.CreateCacheKey());
            var output = default(FasterCacheValue);
            store.Read(ref k, ref output);
            if (output.value.xIsEmpty()) return default;
            return output.value.xToEntity<T>();;
        }

        public T GetCache<T>(CacheOptions<T> options)
        {
            var majorKey = this.CreateCacheKey(options.Keys);
            var subKey = this.CreateCacheKey(options.AdditionalKeys);
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey($"{majorKey}|{subKey}");
            var output = default(FasterCacheValue);
            store.Read(ref k, ref output);
            if (output.value.xIsEmpty()) return default;
            return output.value.xToEntity<T>();;
        }

        public T GetCache<T>(string key)
        {
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey(this.CreateCacheKey(key));
            var output = default(FasterCacheValue);
            store.Read(ref k, ref output);
            if (output.value.xIsEmpty()) return default;
            return output.value.xToEntity<T>();
        }

        public void SetCache<T>(T value, int? expireTimeout)
        {
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey(this.CreateCacheKey());
            var v = new FasterCacheValue(value.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void SetCache<T>(string key, T value, int? expireTimeout)
        {
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey(this.CreateCacheKey(key));
            var v = new FasterCacheValue(value.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void SetCache<T>(string key, T value, DateTimeOffset? expireTimeout)
        {   
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey(this.CreateCacheKey(key));
            var v = new FasterCacheValue(value.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void SetCache<T>(CacheOptions<T> options)
        {
            var majorKey = this.CreateCacheKey(options.Keys);
            var subKey = this.CreateCacheKey(options.AdditionalKeys);
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var k = new FasterCacheKey($"{majorKey}|{subKey}");
            var v = new FasterCacheValue(options.Data.xToJson());
            store.Upsert(ref k, ref v);
        }

        public void RemoveCache(string key)
        {
            var k = new FasterCacheKey(this.CreateCacheKey(key));
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var status = store.Delete(ref k);
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            status.xIfNotEmpty(() => _logger.LogDebug(status.ToString()));
        }

        public void RemoveCache<T>(CacheOptions<T> options)
        {
            var majorKey = this.CreateCacheKey(options.Keys);
            var subKey = this.CreateCacheKey(options.AdditionalKeys);
            var resultKey = new FasterCacheKey($"{majorKey}|{subKey}");
            using var store = _fasterKV.For(new FasterCacheFunctions()).NewSession<FasterCacheFunctions>();
            var status = store.Delete(ref resultKey);
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            status.xIfNotEmpty(() => _logger.LogDebug(status.ToString()));
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Reset<T>()
        {
            throw new NotImplementedException();
        }
    }
}