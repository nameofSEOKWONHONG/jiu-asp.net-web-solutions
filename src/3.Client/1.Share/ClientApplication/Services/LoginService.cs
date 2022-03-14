using System.Text;
using ClientApplication.ViewModel;
using eXtensionSharp;
using InjectionExtension;

namespace ClientApplication.Services;

public interface ILoginService
{
    Task<string> LoginAsync(string email, string password);
}

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeOfInterface:typeof(ILoginService))]
public class LoginService : ILoginService
{
    private readonly IHttpClientFactory _clientFactory;
    public LoginService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var dic = new Dictionary<string, string>()
        {
            { nameof(email), email },
            { nameof(password), password }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post, ClientConst.LOGINASYNC);
        request.Content = new StringContent(dic.xToJson(), Encoding.UTF8, ClientConst.MEDIA_TYPE_JSON);
        
        using var client = _clientFactory.CreateClient(ClientConst.CLIENT_NAME);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}   