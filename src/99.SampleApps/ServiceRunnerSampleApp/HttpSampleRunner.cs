using ClientApplication.Common.Services;
using Domain.Entities;
using eXtensionSharp;
using RestSharp;

namespace ServiceRunnerSampleApp;

public class SampleServiceRunner: ServiceRunnerBase
{
    private TB_TODO _selectedItem;
    private readonly string _baseUrl = "https://localhost:5001";
    
    public SampleServiceRunner(IServiceRunnerBase next) : base(next)
    {
    }
    
    public override async Task<bool> OnExecuteAsync()
    {
        //http request send insert TB_TODO
        var accessToken = string.Empty;
        using var client = new RestClient(_baseUrl);
        client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
        var clientRequest = new RestRequest("v1/api/todo/addTodo", Method.Post);
        clientRequest.Method = Method.Post;
        clientRequest.AddHeader("Accept", "application/json");
        clientRequest.AddParameter("application/json", new TB_TODO().xToJson(),
            ParameterType.RequestBody);
        var response = await client.ExecuteAsync(clientRequest);
        var content = response.Content;
        _selectedItem = content.xToEntity<TB_TODO>();
        if (_selectedItem.ID > 0)
        {
            this.Options.Result.Add("Data", _selectedItem.xToJson());
            return true;
        }
        return false;
    }

    public override async Task OnErrorAsync()
    {
        var accessToken = string.Empty;
        using var client = new RestClient(_baseUrl);
        client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
        var clientRequest = new RestRequest("v1/api/todo/removeTodo", Method.Post);
        clientRequest.Method = Method.Post;
        clientRequest.AddHeader("Accept", "application/json");
        clientRequest.AddParameter("application/json", _selectedItem.xToJson(),
            ParameterType.RequestBody);
        var response = await client.ExecuteAsync(clientRequest);
    }

    public override void Dispose()
    {
        
    }
}