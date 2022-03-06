namespace SpectreConsoleApplication.Menus.Abstract;

public interface IViewBase : IDisposable
{
    void Show();
    bool ViewResult { get; }
}