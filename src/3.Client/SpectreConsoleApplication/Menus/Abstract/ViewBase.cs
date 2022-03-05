using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ViewBase : IViewBase
{
    public bool ViewResult { get; protected set; } = true;
    protected ISession _session;
    protected ILogger _logger;
    public ViewBase(ILogger logger, ISession session)
    {
        _logger = logger;
        _session = session;
    }
    public abstract void Show();

    public virtual void Dispose()
    {
        Console.WriteLine("view dispose");
    }
}
