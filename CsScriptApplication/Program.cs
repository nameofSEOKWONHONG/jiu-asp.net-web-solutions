using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using CSScriptLib;
using eXtensionSharp;

namespace CsScriptApplication
{
    public interface IExecutor<TRequest, TResult>
    {
        TResult Result { get; }
        TRequest Request { set; }
        bool ExecuteBefore();
        void Execute();
        void ExecuteAfter();
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var code = @"
using System;
using CsScriptApplication;
public class ExecutorA : IExecutor<string, string>
{
    public string Request { private get; set; }
    public string Result { get; private set; }
    public bool ExecuteBefore()
    {
        return true;
    }

    public void Execute()
    {
        Result = $""Hey, {this.Request}"";
        Console.WriteLine(""Execute"");
    }

    public void ExecuteAfter()
    {
        Console.WriteLine(""Executed"");
    }
}
";
            var codes = new Dictionary<string, IExecutor<string, string>>();
            codes.Add(code.xToHash(), CSScript.Evaluator.LoadCode<IExecutor<string, string>>(code));

            var executor = codes[code.xToHash()];

            // IExecutor<string> executor = CSScript.Evaluator.LoadCode<IExecutor<string>>(code);
            executor.Request = "Hello World";
            if (executor.ExecuteBefore())
            {
                executor.Execute();
                executor.ExecuteAfter();
                Console.WriteLine(executor.Result);
            }
        }
    }
}
