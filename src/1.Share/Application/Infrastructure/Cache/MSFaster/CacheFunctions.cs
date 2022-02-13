using System;
using System.Diagnostics;
using FASTER.core;

namespace Application.Infrastructure.Cache.MSFaster;

/// <summary>
/// Callback for FASTER operations
/// </summary>
public class FasterCacheFunctions : SimpleFunctions<FasterCacheKey, FasterCacheValue, FasterCacheContext>
{
    public override void ReadCompletionCallback(ref FasterCacheKey key, 
        ref FasterCacheValue input, 
        ref FasterCacheValue output, 
        FasterCacheContext ctx, Status status)
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