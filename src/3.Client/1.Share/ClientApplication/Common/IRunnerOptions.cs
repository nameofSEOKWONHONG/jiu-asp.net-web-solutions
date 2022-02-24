using eXtensionSharp;

namespace ClientApplication.Common.Services;

public interface IRunnerOptions
{
    DynamicDictionary<string> Request { get; set; }
    DynamicDictionary<string> Result { get; set; }
}
