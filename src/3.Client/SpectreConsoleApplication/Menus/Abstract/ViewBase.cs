using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ViewBase : IViewBase
{
    protected ILogger _logger;
    public ViewBase(ILogger logger)
    {
        _logger = logger;
    }
    public abstract void Show();

    public virtual void Dispose()
    {
        Console.WriteLine("view dispose");
    }
}
