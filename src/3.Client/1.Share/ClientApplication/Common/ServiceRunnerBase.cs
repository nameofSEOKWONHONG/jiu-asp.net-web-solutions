
namespace ClientApplication.Common.Services;

public abstract class ServiceRunnerBase : IServiceRunnerBase
{
    public IRunnerOptions Options { get; set; }
    public IServiceRunnerBase Next { get; private set; }
    protected ServiceRunnerBase(IServiceRunnerBase next = null)
    {
        if (next is null) return;
        this.Next = next;
    }

    public abstract Task<bool> OnExecuteAsync();
    public abstract Task OnErrorAsync();

    public abstract void Dispose();
}