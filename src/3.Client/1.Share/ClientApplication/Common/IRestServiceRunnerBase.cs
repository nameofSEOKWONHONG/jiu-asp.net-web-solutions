using eXtensionSharp;

namespace ClientApplication.Common.Services;

public interface IRestServiceRunnerBase : IDisposable
{
    IRunnerOptions Options { get; set; }
    IRestServiceRunnerBase Next { get; }
    Task<bool> OnExecuteAsync();
    Task OnErrorAsync();
}