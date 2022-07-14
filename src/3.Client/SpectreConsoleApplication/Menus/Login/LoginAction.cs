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

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(ILoginAction))]
public class LoginAction : ActionBase, ILoginAction
{
    private readonly ILoginService _loginService;
    public LoginAction(ILogger<LoginAction> logger, 
        IContextBase contextBase,
        IHttpClientFactory clientFactory,
        ILoginService service) : base(logger, contextBase, clientFactory)
    {
        _loginService = service;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        return await _loginService.LoginAsync(email, password);
    }
}