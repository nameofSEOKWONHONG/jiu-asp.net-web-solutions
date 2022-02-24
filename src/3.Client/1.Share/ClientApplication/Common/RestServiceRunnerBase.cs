using eXtensionSharp;

namespace ClientApplication.Common.Services;

public abstract class RestServiceRunnerBase : IRestServiceRunnerBase
{
    public IRunnerOptions Options { get; set; }
    public IRestServiceRunnerBase Next { get; private set; }
    protected RestServiceRunnerBase(IRestServiceRunnerBase next)
    {
        if (next is null) return;
        this.Next = next;
    }

    public abstract Task<bool> OnExecuteAsync();
    public abstract Task OnErrorAsync();

    public abstract void Dispose();
}