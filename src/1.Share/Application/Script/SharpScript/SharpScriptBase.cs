using System.Threading.Tasks;

namespace Application.Script.SharpScript;

public interface ISharpScriptBase
{
    Task ExecuteAsync();
    void Execute();
}

public abstract class SharpScriptBase<TOptions, TRequest> : ISharpScriptBase
{
    public TOptions Options { get; set; }
    public TRequest Request { get; set; }
    public abstract Task ExecuteAsync();

    public virtual void Execute()
    {
    }
}

public abstract class SharpScriptBase<TOptions, TRequest, TResult> : SharpScriptBase<TOptions, TRequest>
{
    public TResult Result { get; protected set; }
}

