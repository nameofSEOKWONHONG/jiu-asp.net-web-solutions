using Domain.Entities;
using eXtensionSharp;
using RestSharp;

namespace ClientApplication.Services;

public interface IRestServiceRunner
{
    IRestServiceRunner Next { get; }
    Task<bool> OnSuccessedAsync();
    Task OnFailedAsync();
}

public abstract class RestServiceRunner<TRequest, TResult> : IRestServiceRunner
{
    public TRequest Request { get; set; }
    public TResult Result { get; private set; }
    public IRestServiceRunner Next { get; private set; }
    public RestServiceRunner(RestServiceRunner<TRequest, TResult> next = null)
    {
        this.Next = next;
    }

    public abstract Task<bool> OnSuccessedAsync();
    public abstract Task OnFailedAsync();
}

public class ServiceRunnerCore
{
    private readonly IRestServiceRunner _runner;
    public ServiceRunnerCore(IRestServiceRunner runner)
    {
        _runner = runner;
    }

    public async Task RunAsync()
    {
        var runner = _runner;
        while (true)
        {
            if (await _runner.OnSuccessedAsync())
            {
                await _runner.OnFailedAsync();
                break;
            }    
            if(runner.xIsEmpty()) break;
            runner = runner.Next;
        }
    }
}

public class SampleRestServiceRunner: RestServiceRunner<TB_TODO, bool>
{
    private TB_TODO _selectedItem;
    
    public SampleRestServiceRunner(SampleRestServiceRunner runner) : base(runner)
    {
    }
    
    public override async Task<bool> OnSuccessedAsync()
    {
        //http request send insert TB_TODO
        var url = "https://localhost:5001/v1/api/todo/addTodo";
        var client = new RestClient(url);
        client.AddDefaultHeader("Authorization", $"Bearer {"access token"}");
        var clientRequest = new RestRequest();
        clientRequest.Method = Method.Post;
        clientRequest.AddHeader("Accept", "application/json");
        clientRequest.AddParameter("application/json", new TB_TODO().xToJson(),
            ParameterType.RequestBody);
        var response = await client.ExecuteAsync(clientRequest);
        var content = response.Content;
        _selectedItem = content.xToEntity<TB_TODO>();
        if (_selectedItem.ID > 0) return true;
        return false;
    }

    public override async Task OnFailedAsync()
    {
        var url = "https://localhost:5001/v1/api/todo/removeTodo";
        var client = new RestClient(url);
        client.AddDefaultHeader("Authorization", $"Bearer {"access token"}");
        var clientRequest = new RestRequest();
        clientRequest.Method = Method.Post;
        clientRequest.AddHeader("Accept", "application/json");
        clientRequest.AddParameter("application/json", _selectedItem.xToJson(),
            ParameterType.RequestBody);
        var response = await client.ExecuteAsync(clientRequest);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var serviceCore = new ServiceRunnerCore(
            new SampleRestServiceRunner(
                new SampleRestServiceRunner(
                    new SampleRestServiceRunner(null)
                    )
                )
            );

        serviceCore.RunAsync().GetAwaiter().GetResult();
    }
}