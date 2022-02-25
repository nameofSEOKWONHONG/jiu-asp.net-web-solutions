using System.Net.Http.Headers;
using Domain.Entities;
using LiteDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Member;

public class MemberAction : ActionBase
{
    public MemberAction(ILogger<MemberAction> logger, IHttpClientFactory clientFactory) : base(logger, clientFactory)
    {
    }

    public IEnumerable<TB_USER> GetMembers()
    {   
        using var client = _clientFactory.CreateClient(AppConst.HTTP_NAME);
        var request =
            new HttpRequestMessage(HttpMethod.Get, "api/v1/Users/GetAll/1/10");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", AppConst.ACESS_TOKEN);        
        var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
        var items = JsonConvert.DeserializeObject<IEnumerable<TB_USER>>(result, settings);
        return items;
    }
}