// using System;
// using System.Diagnostics;
// using System.Threading.Tasks;
// using Application.Script.SharpScript;
// using Infrastructure.Persistence;
//
// public class LoopSampleScript :SharpScriptBase<bool, bool, string>
// {
//     public override Task ExecuteAsync()
//     {
//         throw new NotImplementedException();
//     }
//
//     public override void Execute()
//     {
//         var sw = new Stopwatch();
//         
//         sw.Start();
//
//         var total = 0;
//         for (var i = 0; i < 1000000; i++)
//         {
//             total += 1;
//         }
//         
//         sw.Stop();
//
//         this.Result = sw.Elapsed.ToString();
//     }
// }