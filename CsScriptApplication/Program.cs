using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Transactions;
using CSScriptLib;
using eXtensionSharp;

namespace CsScriptApplication
{
    
    
    public class Program
    {
        public static void Main(string[] args)
        {
            // var sample = new CsScriptSample();
            // sample.Run();

            var sample = new ClearScriptSample();
            sample.SqlRun();
        }
    }
}
