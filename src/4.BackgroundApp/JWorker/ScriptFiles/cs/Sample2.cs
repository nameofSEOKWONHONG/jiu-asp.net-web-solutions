using System;
using System.Threading.Tasks;
using Application.Script.SharpScript;

public class Sample2 : SharpScriptBase<string, string>
{
    public override Task ExecuteAsync()
    {
        Console.WriteLine("Sample2 Run");
        return Task.CompletedTask;
    }
}