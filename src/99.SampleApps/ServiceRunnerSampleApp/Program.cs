using System.Diagnostics;
using ClientApplication.Common.Services;
using eXtensionSharp;
using ServiceRunnerSampleApp;

var list = new List<string>();
Enumerable.Range(1, 1000).ToList().ForEach(i =>
{
    list.Add(i.ToString());
});

var span = new ReadOnlySpan<string>(list.ToArray());

Stopwatch sw = new Stopwatch();

for (var i = 0; i < 10; i++)
{
    sw.Start();
    foreach (var s in span)
    {
        var j = 1 + 1;
    }
    sw.Stop();
    Console.WriteLine($"span : {sw.Elapsed.ToString()}");

    sw.Reset();
    sw.Start();
    foreach (var s in list)
    {
        var j = 1 + 1;
    }
    sw.Stop();
    Console.WriteLine($"list : {sw.Elapsed.ToString()}");
    
}

// var serviceCore = new ServiceRunnerCore(
//     new SampleServiceRunnerA(
//         new SampleServiceRunnerB() 
//         ),
//     new RunnerOptions
//     {
//         Request = new DynamicDictionary<string>
//         {
//             {"accessToken", "asdf88njnjnafhsgbdjhbjh74"}
//         }
//     });
//
// await serviceCore.RunAsync();
//
// var serviceCore2 = new ServiceParallelRunnerCore(new ServiceRunnerBase[]
// {
//     new SampleServiceRunnerA() { 
//         Options = new RunnerOptions() {
//             Request = new DynamicDictionary<string>() 
//             {
//                 {"accessToken", "asdf88njnjnafhsgbdjhbjh74"}
//             }
//         }
//     },
//     new SampleServiceRunnerB() { 
//         Options = new RunnerOptions() { 
//             Request = new DynamicDictionary<string>()
//             {
//                 {"RunnerB1", "Hello"}, {"RunnerB2", "World"},
//             }
//         }
//     }
// });
//
// await serviceCore2.RunAsync();



