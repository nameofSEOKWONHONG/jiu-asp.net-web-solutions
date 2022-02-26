using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public interface IViewBase : IDisposable
{
    void Show();
}