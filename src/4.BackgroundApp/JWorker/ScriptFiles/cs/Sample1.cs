using System;
using System.Threading.Tasks;
using Application.Script.SharpScript;

public class Sample1 : SharpScriptBase<string, string>
{
    public override Task ExecuteAsync()
    {
        Console.WriteLine("Execute Sample1");
        return Task.CompletedTask;
    }
}