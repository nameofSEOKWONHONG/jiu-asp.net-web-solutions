using eXtensionSharp;

namespace ClientApplication.Common.Services;

public interface IServiceRunnerBase : IDisposable
{
    IRunnerOptions Options { get; set; }
    IServiceRunnerBase Next { get; }
    Task<bool> OnExecuteAsync();
    Task OnErrorAsync();
}