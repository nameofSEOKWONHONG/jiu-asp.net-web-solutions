using System.Net.Http.Headers;
using ClientApplication.ViewModel;
using Domain.Base;
using Domain.Entities;
using InjectionExtension;
using Newtonsoft.Json;

namespace ClientApplication.Services;

public interface IMemberService
{
    Task<IEnumerable<TB_USER>> GetMembersAsync();
}

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IMemberService))]
public class MemberService : IMemberService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IClientSession _clientSession;
    public MemberService(IHttpClientFactory clientFactory, IClientSession clientSession)
    {
        this._clientFactory = clientFactory;
        this._clientSession = clientSession;
    }
    
    public async Task<IEnumerable<TB_USER>> GetMembersAsync()
    {
        using var client = _clientFactory.CreateClient(ClientConst.CLIENT_NAME);
        var request =
            new HttpRequestMessage(HttpMethod.Get, "api/v1/Users/GetAll/1/10");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _clientSession.AccessToken);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
        var items = JsonConvert.DeserializeObject<IEnumerable<TB_USER>>(result, settings);
        return items;
    }
}