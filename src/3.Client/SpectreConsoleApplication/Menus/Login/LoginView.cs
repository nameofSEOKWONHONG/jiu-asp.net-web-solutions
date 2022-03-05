using Application.Infrastructure.Injection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Menus.Login;

namespace SpectreConsoleApplication.Menus;


[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton)]
public class LoginView : ViewBase
{
    public string AccessToken { get; private set; }

    private readonly ILoginAction _action;
    public LoginView(ILogger<LoginView> logger,
        ISession session,
        ILoginAction action) : base(logger, session)
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