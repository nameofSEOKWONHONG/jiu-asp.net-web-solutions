using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class MenuViewBase : IMenuViewBase
{
    protected ILogger _logger;
    public MenuViewBase(ILogger logger)
    {
        _logger = logger;
    }
    public abstract void Show();
}
