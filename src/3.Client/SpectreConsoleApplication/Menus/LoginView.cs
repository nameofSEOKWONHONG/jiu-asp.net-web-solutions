using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus;

public class LoginView : MenuViewBase
{
    public string AccessToken { get; private set; }

    private readonly LoginAction _action;
    public LoginView(ILogger<LoginView> logger, LoginAction action) : base(logger)
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

        var email = AnsiConsole.Ask<string>("What's your [green]email[/]?");
        AnsiConsole.WriteLine($"email is {email}");

        var password =  AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());
        
        AccessToken = _action.LoginAsync(email, password).GetAwaiter().GetResult();
    }
}