using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ClearScript.V8;

namespace CsScriptApplication;

public class ClearScriptSample
{
    public ClearScriptSample()
    {
        
    }

    public void Run()
    {
        using var runtime = new V8Runtime();
        // using var engine1 = runtime.CreateScriptEngine();
        // using var engine2 = runtime.CreateScriptEngine();
        //
        // engine1.Execute("foo = {bar : 123}");
        // engine2.Script.foo = engine1.Script.foo;
        // Console.WriteLine($"engine1 foo : {engine1.Script.foo.bar}");
        // Console.WriteLine($"engine2 foo : {engine2.Script.foo.bar}");

//         using var engine = runtime.CreateScriptEngine();
//         engine.AddHostTypes(new[] {typeof(int), typeof(Enumerable)});
//         engine.ExecuteCommand(@"
// var empty = Enumerable.Empty(Int32);
// var numbers = Enumerable.Range(0, 10).ToArray();
// ");
//         Console.WriteLine(engine.Script.empty);
//         var numbers = (Int32[])engine.Script.numbers;
//         foreach (var number in numbers)
//         {
//             Console.WriteLine(number);
//         }

        var maps = new Dictionary<string, string>()
        {
            { "[A]", "10" },
            { "[B]", "20" },
            { "[C]", "HELLO"}
        };
        var text = "'[C]:' + ([A] + [B])";
        foreach (var keyValuePair in maps)
        {
            text = text.Replace(keyValuePair.Key, keyValuePair.Value);
        }
        using var engine = runtime.CreateScriptEngine();
        engine.Execute($"var result = {text}");
        Console.WriteLine(engine.Script.result);
    }
}