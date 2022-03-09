using ClientApplication.Services;
using Domain.Base;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Login;

public interface ILoginAction
{
    Task<string> LoginAsync(string email, string password);
}

[ServiceLifeTime(ENUM_LIFE_TYPE.Scope, typeof(ILoginAction))]
public class LoginAction : ActionBase, ILoginAction
{
    private readonly ILoginService _loginService;
    public LoginAction(ILogger<LoginAction> logger, 
        IClientSession clientSession,
        IHttpClientFactory clientFactory,
        ILoginService service) : base(logger, clientSession, clientFactory)
    {
        _loginService = service;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        return await _loginService.LoginAsync(email, password);
    }
}