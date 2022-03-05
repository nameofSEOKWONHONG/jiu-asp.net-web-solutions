using System.Text;
using Application.Infrastructure.Injection;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Login;

public interface ILoginAction
{
    Task<string> LoginAsync(string email, string password);
}

[ServiceLifeTime(ENUM_LIFE_TYPE.Scope, typeof(ILoginAction))]
public class LoginAction : ActionBase, ILoginAction
{
    public LoginAction(ILogger<LoginAction> logger, 
        ISession session,
        IHttpClientFactory clientFactory) : base(logger, session, clientFactory)
    {
    }

    public async Task<string> LoginAsync(string email, string password)
    { 
        var dic = new Dictionary<string, string>()
        {
            { nameof(email), email },
            { nameof(password), password }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post,
            "api/v1/Account/SignIn");
        request.Content = new StringContent(dic.xToJson(), Encoding.UTF8, "application/json");
        
        using var client = _clientFactory.CreateClient(AppConst.HTTP_NAME);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}