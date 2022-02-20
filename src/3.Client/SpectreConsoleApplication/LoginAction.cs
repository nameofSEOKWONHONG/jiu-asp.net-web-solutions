using System.Text;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SpectreConsoleApplication;

public class LoginAction
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpFactory;
    public LoginAction(ILogger<LoginAction> logger, 
        IHttpClientFactory httpFactory)
    {
        _logger = logger;
        _httpFactory = httpFactory;
    }

    public async Task<string> LoginAsync(string email, string password)
    { 
        var dic = new Dictionary<string, string>()
        {
            { nameof(email), email },
            { nameof(password), password }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://localhost:5001/api/v1/Account/SignIn");
        request.Content = new StringContent(dic.xToJson(), Encoding.UTF8, "application/json");
        
        using var client = _httpFactory.CreateClient();
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}

public class LoginView
{
    private readonly LoginAction _action;
    public LoginView(LoginAction action)
    {
        _action = action;
    }

    public bool Login(out string token)
    {
        token = string.Empty;
        
        if (!AnsiConsole.Confirm("Hello, Are you ready?"))
        {
            AnsiConsole.MarkupLine("Ok... :(");
            return false;
        }

        var email = AnsiConsole.Ask<string>("What's your [green]email[/]?");
        AnsiConsole.WriteLine($"email is {email}");

        var password =  AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());
        
        token = _action.LoginAsync(email, password).GetAwaiter().GetResult();

        return true;
    }
}