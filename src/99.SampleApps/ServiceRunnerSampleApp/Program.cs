using ClientApplication.Common.Services;
using Domain.Entities;
using eXtensionSharp;
using RestSharp;

Console.WriteLine("Hello, World!");

var serviceCore = new RestServiceRunnerCore(
    new SampleRestServiceRunnerA(
        new SampleRestServiceRunnerB(null) 
        ),
    new RunnerOptions
    {
        Request = new DynamicDictionary<string>
        {
            {"accessToken", "asdf88njnjnafhsgbdjhbjh74"}
        }
    });

await serviceCore.RunAsync();

public class SampleRestServiceRunnerA : RestServiceRunnerBase
{
    public SampleRestServiceRunnerA(IRestServiceRunnerBase next) : base(next)
    {
    }
    
    public override Task<bool> OnExecuteAsync()
    {
        Console.WriteLine($"AccessToken : {this.Options.Request["accessToken"]}");
        Console.WriteLine("A");
        this.Options.Result.Add("A1", "Hello");
        this.Options.Result.Add("A2", "World");
        return Task.FromResult(true);
    }

    public override Task OnErrorAsync()
    {
        Console.WriteLine("A Failed");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        
    }
}

public class SampleRestServiceRunnerB : RestServiceRunnerBase
{
    public SampleRestServiceRunnerB(IRestServiceRunnerBase next) : base(next)
    {
    }
    
    public override Task<bool> OnExecuteAsync()
    {
        Console.WriteLine("B");
        this.Options.Request.xForEach(kv =>
        {
            Console.WriteLine("Parameters:");
            Console.WriteLine(kv.Key);
            Console.WriteLine(kv.Value);
        });
        return Task.FromResult(false);
    }

    public override Task OnErrorAsync()
    {
        Console.WriteLine("B Failed");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        
    }
}

public class SampleRestServiceRunner: RestServiceRunnerBase
{
    private TB_TODO _selectedItem;
    private readonly string _baseUrl = "https://localhost:5001";
    
    public SampleRestServiceRunner(IRestServiceRunnerBase next) : base(next)
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