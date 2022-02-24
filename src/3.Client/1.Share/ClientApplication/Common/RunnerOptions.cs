using eXtensionSharp;

namespace ClientApplication.Common.Services;

public class RunnerOptions : IRunnerOptions
{
    public DynamicDictionary<string> Request { get; set; }
    public DynamicDictionary<string> Result { get; set; }
}