using System;
using System.Collections.Generic;
using System.Transactions;
using CSScriptLib;
using eXtensionSharp;

namespace CsScriptApplication;

public class CsScriptSample
{
    public CsScriptSample()
    {
        
    }

    public void Run()
    {
        var codeItems = new List<string>();
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

    public bool ModelValidator() {
        return true;
    }

    public bool BizValidator() {
        return true;
    }
}
";
        var codes = new Dictionary<string, IExecutor<string, string>>();
            
        codeItems.Add(code);
        codeItems.ForEach(item =>
        {
            codes.Add(item.xToHash(), CSScript.Evaluator.LoadCode<IExecutor<string, string>>(item));    
        });

        using (var tran = new TransactionScope(TransactionScopeOption.Required))
        {
            codes.xForEach(item =>
            {
                var executor = codes[code.xToHash()];

                // IExecutor<string> executor = CSScript.Evaluator.LoadCode<IExecutor<string>>(code);
                executor.Request = "Hello World";

                executor.ModelValidator();
                executor.BizValidator();
            
                if (executor.ExecuteBefore())
                {
                    executor.Execute();
                    executor.ExecuteAfter();
                    Console.WriteLine(executor.Result);
                }
            });
                
            tran.Complete();
        }
    }
}

public interface IExecutor<TRequest, TResult>
{
    TResult Result { get; }
    TRequest Request { set; }
    bool ExecuteBefore();
    void Execute();
    void ExecuteAfter();
    /// <summary>
    /// throw exception
    /// </summary>
    /// <returns></returns>
    bool BizValidator();
    /// <summary>
    /// throw exception
    /// </summary>
    /// <returns></returns>
    bool ModelValidator();
}