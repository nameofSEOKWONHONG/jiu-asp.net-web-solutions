using System.Diagnostics;
using ClientApplication.Common.Services;
using eXtensionSharp;
using ServiceRunnerSampleApp;

var serviceCore = new ServiceRunnerCore(
    new SampleServiceRunnerA(
        new SampleServiceRunnerB() 
        ),
    new RunnerOptions
    {
        Request = new DynamicDictionary<string>
        {
            {"accessToken", "asdf88njnjnafhsgbdjhbjh74"}
        }
    });

await serviceCore.RunAsync();

var serviceCore2 = new ServiceParallelRunnerCore(new ServiceRunnerBase[]
{
    new SampleServiceRunnerA() { 
        Options = new RunnerOptions() {
            Request = new DynamicDictionary<string>() 
            {
                {"accessToken", "asdf88njnjnafhsgbdjhbjh74"}
            }
        }
    },
    new SampleServiceRunnerB() { 
        Options = new RunnerOptions() { 
            Request = new DynamicDictionary<string>()
            {
                {"RunnerB1", "Hello"}, {"RunnerB2", "World"},
            }
        }
    }
});

await serviceCore2.RunAsync();



