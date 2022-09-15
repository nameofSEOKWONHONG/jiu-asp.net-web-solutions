// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using ZstdNetSample;

Console.WriteLine("Benchmark Start");

#if DEBUG
var benchmark = new ZstdBenchmark();
benchmark.Runner();
#else
//BenchmarkRunner.Run<ZstdBenchmark>();
BenchmarkRunner.Run<Lz4Benchmark>();
#endif

Console.WriteLine("Benchmark End");
/*
Benchmark Start
// Validating benchmarks:
// ***** BenchmarkRunner: Start   *****
// ***** Found 1 benchmark(s) in total *****
// ***** Building 1 exe(s) in Parallel: Start   *****
// start dotnet  restore /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1 /p:Deterministic=true /p:Optimize=true in D:\workspace\jiu-asp.net-web-solutions\src\99.SampleApps\ZstdNetSample\bin\Release\net6.0\11dc6fe0-a12e-47dd-a188-0b6940b50dbf
// command took 0.49s and exited with 0
// start dotnet  build -c Release --no-restore /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1 /p:Deterministic=true /p:Optimize=true in D:\workspace\jiu-asp.net-web-solutions\src\99.SampleApps\ZstdNetSample\bin\Release\net6.0\11dc6fe0-a12e-47dd-a188-0b6
940b50dbf
// command took 1.72s and exited with 0
// ***** Done, took 00:00:02 (2.28 sec)   *****
// Found 1 benchmarks:
//   ZstdBenchmark.Runner: DefaultJob

Setup power plan (GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c FriendlyName: 고성능)
// **************************
// Benchmark: ZstdBenchmark.Runner: DefaultJob
// *** Execute ***
// Launch: 1 / 1
// Execute: dotnet 11dc6fe0-a12e-47dd-a188-0b6940b50dbf.dll --benchmarkName ZstdNetSample.ZstdBenchmark.Runner --job Default --benchmarkId 0 in D:\workspace\jiu-asp.net-web-solutions\src\99.SampleApps\ZstdNetSample\bin\Release\net6.0\11dc6fe0-a12e-47dd-a188-0b6940b5
0dbf\bin\Release\net6.0
// BeforeAnythingElse

// Benchmark Process Environment Information:
// Runtime=.NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2
// GC=Concurrent Workstation
// HardwareIntrinsics=AVX2,AES,BMI1,BMI2,FMA,LZCNT,PCLMUL,POPCNT VectorSize=256
// Job: DefaultJob

OverheadJitting  1: 1 op, 146700.00 ns, 146.7000 us/op
WorkloadJitting  1: 1 op, 339761700.00 ns, 339.7617 ms/op

OverheadJitting  2: 1 op, 200.00 ns, 200.0000 ns/op
WorkloadJitting  2: 1 op, 337652300.00 ns, 337.6523 ms/op

WorkloadWarmup   1: 1 op, 340627700.00 ns, 340.6277 ms/op
WorkloadWarmup   2: 1 op, 338069300.00 ns, 338.0693 ms/op
WorkloadWarmup   3: 1 op, 341358900.00 ns, 341.3589 ms/op
WorkloadWarmup   4: 1 op, 342666500.00 ns, 342.6665 ms/op
WorkloadWarmup   5: 1 op, 342857500.00 ns, 342.8575 ms/op
WorkloadWarmup   6: 1 op, 333416200.00 ns, 333.4162 ms/op
WorkloadWarmup   7: 1 op, 339517300.00 ns, 339.5173 ms/op
WorkloadWarmup   8: 1 op, 341446100.00 ns, 341.4461 ms/op
WorkloadWarmup   9: 1 op, 331485900.00 ns, 331.4859 ms/op

// BeforeActualRun
WorkloadActual   1: 1 op, 336338000.00 ns, 336.3380 ms/op
WorkloadActual   2: 1 op, 338829600.00 ns, 338.8296 ms/op
WorkloadActual   3: 1 op, 337941300.00 ns, 337.9413 ms/op
WorkloadActual   4: 1 op, 336445000.00 ns, 336.4450 ms/op
WorkloadActual   5: 1 op, 339391900.00 ns, 339.3919 ms/op
WorkloadActual   6: 1 op, 331477100.00 ns, 331.4771 ms/op
WorkloadActual   7: 1 op, 338761200.00 ns, 338.7612 ms/op
WorkloadActual   8: 1 op, 342087300.00 ns, 342.0873 ms/op
WorkloadActual   9: 1 op, 336084000.00 ns, 336.0840 ms/op
WorkloadActual  10: 1 op, 340953800.00 ns, 340.9538 ms/op
WorkloadActual  11: 1 op, 341496400.00 ns, 341.4964 ms/op
WorkloadActual  12: 1 op, 336606000.00 ns, 336.6060 ms/op
WorkloadActual  13: 1 op, 337587100.00 ns, 337.5871 ms/op
WorkloadActual  14: 1 op, 339411600.00 ns, 339.4116 ms/op
WorkloadActual  15: 1 op, 333165000.00 ns, 333.1650 ms/op

// AfterActualRun
WorkloadResult   1: 1 op, 336338000.00 ns, 336.3380 ms/op
WorkloadResult   2: 1 op, 338829600.00 ns, 338.8296 ms/op
WorkloadResult   3: 1 op, 337941300.00 ns, 337.9413 ms/op
WorkloadResult   4: 1 op, 336445000.00 ns, 336.4450 ms/op
WorkloadResult   5: 1 op, 339391900.00 ns, 339.3919 ms/op
WorkloadResult   6: 1 op, 331477100.00 ns, 331.4771 ms/op
WorkloadResult   7: 1 op, 338761200.00 ns, 338.7612 ms/op
WorkloadResult   8: 1 op, 342087300.00 ns, 342.0873 ms/op
WorkloadResult   9: 1 op, 336084000.00 ns, 336.0840 ms/op
WorkloadResult  10: 1 op, 340953800.00 ns, 340.9538 ms/op
WorkloadResult  11: 1 op, 341496400.00 ns, 341.4964 ms/op
WorkloadResult  12: 1 op, 336606000.00 ns, 336.6060 ms/op
WorkloadResult  13: 1 op, 337587100.00 ns, 337.5871 ms/op
WorkloadResult  14: 1 op, 339411600.00 ns, 339.4116 ms/op
WorkloadResult  15: 1 op, 333165000.00 ns, 333.1650 ms/op
GC:  0 0 0 5247344 1
Threading:  0 0 1

// AfterAll
// Benchmark Process 22204 has exited with code 0.

Mean = 337.772 ms, StdErr = 0.753 ms (0.22%), N = 15, StdDev = 2.915 ms
Min = 331.477 ms, Q1 = 336.392 ms, Median = 337.941 ms, Q3 = 339.402 ms, Max = 342.087 ms
IQR = 3.010 ms, LowerFence = 331.876 ms, UpperFence = 343.917 ms
ConfidenceInterval = [334.655 ms; 340.888 ms] (CI 99.9%), Margin = 3.116 ms (0.92% of Mean)
Skewness = -0.49, Kurtosis = 2.49, MValue = 2

// ** Remained 0 (0.0%) benchmark(s) to run. Estimated finish 2022-09-16 0:52 (0h 0m from now) **
Successfully reverted power plan (GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c FriendlyName: 고성능)
// ***** BenchmarkRunner: Finish  *****

// * Export *
  BenchmarkDotNet.Artifacts\results\ZstdNetSample.ZstdBenchmark-report.csv
  BenchmarkDotNet.Artifacts\results\ZstdNetSample.ZstdBenchmark-report-github.md
  BenchmarkDotNet.Artifacts\results\ZstdNetSample.ZstdBenchmark-report.html

// * Detailed results *
ZstdBenchmark.Runner: DefaultJob
Runtime = .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2; GC = Concurrent Workstation
Mean = 337.772 ms, StdErr = 0.753 ms (0.22%), N = 15, StdDev = 2.915 ms
Min = 331.477 ms, Q1 = 336.392 ms, Median = 337.941 ms, Q3 = 339.402 ms, Max = 342.087 ms
IQR = 3.010 ms, LowerFence = 331.876 ms, UpperFence = 343.917 ms
ConfidenceInterval = [334.655 ms; 340.888 ms] (CI 99.9%), Margin = 3.116 ms (0.92% of Mean)
Skewness = -0.49, Kurtosis = 2.49, MValue = 2
-------------------- Histogram --------------------
[329.926 ms ; 343.639 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-rc.1.22431.12
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2


| Method |     Mean |   Error |  StdDev | Completed Work Items | Lock Contentions | Allocated |
|------- |---------:|--------:|--------:|---------------------:|-----------------:|----------:|
| Runner | 337.8 ms | 3.12 ms | 2.92 ms |                    - |                - |      5 MB |

// * Hints *
Outliers
  ZstdBenchmark.Runner: Default -> 1 outlier  was  detected (331.48 ms)

// * Legends *
  Mean                 : Arithmetic mean of all measurements
  Error                : Half of 99.9% confidence interval
  StdDev               : Standard deviation of all measurements
  Completed Work Items : The number of work items that have been processed in ThreadPool (per single operation)
  Lock Contentions     : The number of times there was contention upon trying to take a Monitor's lock (per single operation)
  Allocated            : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms                 : 1 Millisecond (0.001 sec)

// * Diagnostic Output - MemoryDiagnoser *

// * Diagnostic Output - ThreadingDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:00:09 (9.36 sec), executed benchmarks: 1

Global total time: 00:00:11 (11.84 sec), executed benchmarks: 1
// * Artifacts cleanup *
Benchmark End

Process finished with exit code 0.
*/