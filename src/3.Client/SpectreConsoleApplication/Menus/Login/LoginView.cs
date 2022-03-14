using Domain.Base;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Menus.Login;

namespace SpectreConsoleApplication.Menus;


[AddService(ENUM_LIFE_TIME_TYPE.Singleton)]
public class LoginView : ViewBase
{
    public string AccessToken { get; private set; }

    private readonly ILoginAction _action;
    public LoginView(ILogger<LoginView> logger,
        IClientSession clientSession,
        ILoginAction action) : base(logger, clientSession)
    {
        _action = action;
    }

    public override void Show()
    {        
        if (!AnsiConsole.Confirm("Hello, Are you ready?"))
        {
            AnsiConsole.MarkupLine("Ok... :(");
            return;
        }

        var email = AnsiConsole.Ask<string>("Enter your [green]email[/]:");
        AnsiConsole.WriteLine($"email is {email}");

        var password =  AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());
        
        AccessToken = _action.LoginAsync(email, password).GetAwaiter().GetResult();
    }
}