using Domain.Base;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ViewBase : IViewBase
{
    public bool ViewResult { get; protected set; } = true;
    protected IContextBase ContextBase;
    protected ILogger _logger;
    public ViewBase(ILogger logger, IContextBase contextBase)
    {
        _logger = logger;
        ContextBase = contextBase;
    }

    public abstract void Show();

    public virtual void Dispose()
    {
        Console.WriteLine("view dispose");
    }
}


public sealed class ViewBaseCore
{
    private readonly IViewBase _viewBase;
    public ViewBaseCore(IViewBase view)
    {
        _viewBase = view;
    }

    public void Show()
    {
        CONTINUE:
        _viewBase.Show();
        var yn = AnsiConsole.Ask<string>("continue(Y/N) :", "Y");
        if (yn.ToUpper() == "Y") goto CONTINUE;
    }
}