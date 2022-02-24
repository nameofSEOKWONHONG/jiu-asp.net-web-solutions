using ClientApplication.Common.Services;
using eXtensionSharp;

namespace ServiceRunnerSampleApp;

public class SampleServiceRunnerA : ServiceRunnerBase
{
    public SampleServiceRunnerA(IServiceRunnerBase next = null) : base(next)
    {
    }
    
    public override Task<bool> OnExecuteAsync()
    {
        Console.WriteLine("A OnExecuteAsync");
        Console.WriteLine($"AccessToken : {this.Options.Request["accessToken"]}");
        this.Options.Result.Add("A1", "Hello");
        this.Options.Result.Add("A2", "World");
        Console.WriteLine(this.Options.Result.xToJson());
        return Task.FromResult(true);
    }

    public override Task OnErrorAsync()
    {
        Console.WriteLine("A OnErrorAsync");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        
    }
}

public class SampleServiceRunnerB : ServiceRunnerBase
{
    public SampleServiceRunnerB(IServiceRunnerBase next = null) : base(next)
    {
    }
    
    public override Task<bool> OnExecuteAsync()
    {
        Console.WriteLine("B OnExecuteAsync");
        Console.WriteLine("Request Parameters:");
        this.Options.Request.xForEach(kv =>
        {
            Console.WriteLine(kv.Key);
            Console.WriteLine(kv.Value);
        });
        return Task.FromResult(false);
    }

    public override Task OnErrorAsync()
    {
        Console.WriteLine("B OnErrorAsync");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        
    }
}