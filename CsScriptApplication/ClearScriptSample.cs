using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using eXtensionSharp;
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

        // var maps = new Dictionary<string, string>()
        // {
        //     { "[A]", "10" },
        //     { "[B]", "20" },
        //     { "[C]", "HELLO"}
        // };
        // var text = "'[C]:' + ([A] + [B])";
        // foreach (var keyValuePair in maps)
        // {
        //     text = text.Replace(keyValuePair.Key, keyValuePair.Value);
        // }
        // using var engine = runtime.CreateScriptEngine();
        // engine.Execute($"var result = {text}");
        // Console.WriteLine(engine.Script.result);

        using var engine = runtime.CreateScriptEngine();
        engine.AddHostTypes(new[] {typeof(Sample)});
        
        engine.Execute(@"
var sample = new Sample(10, `test`);
var id = sample.Id;
var name = sample.Name;

var toString = function(id, name) {
    return id + ', ' + name;
}

var result = toString(id, name);
");
        Console.WriteLine($"var id = {engine.Script.id}");
        Console.WriteLine($"var name = {engine.Script.name}");
        Console.WriteLine($"var result = {engine.Script.result}");
    }
    
    public class Sample
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public Sample(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    public void SqlRun()
    {
        var maps = new Dictionary<string, string>();
        maps.Add("v_date", "20211231");
        maps.Add("v_id", null);
        
        var query = "TB_TODO_VIEW.js".xFileReadAllText();
        var sb = new StringBuilder();
        maps.xForEach(item =>
        {
            sb.AppendLine($"var {item.Key} = '{item.Value}';");            
        });

        var prefix = sb.ToString();
        var result = $"{prefix} {query}";
        
        using var runtime = new V8Runtime();
        var engine = runtime.CreateScriptEngine();
        engine.Execute(result);
        Console.WriteLine(engine.Script.mainSql);
    }
}