using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus;

public interface IMenuViewBase
{
    void Show();
}


public abstract class MenuViewBaseBase : IMenuViewBase
{
    protected ILogger _logger;
    public MenuViewBaseBase(ILogger logger)
    {
        _logger = logger;
    }
    public abstract void Show();
}