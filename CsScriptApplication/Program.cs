using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Transactions;
using CSScriptLib;
using eXtensionSharp;
using Microsoft.Data.SqlClient;

namespace CsScriptApplication
{
    
    
    public class Program
    {
        public static void Main(string[] args)
        {
            // var sample = new CsScriptSample();
            // sample.Run();

            // var sample = new ClearScriptSample();
            // sample.SqlRun();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("V_DATE", "20220101"));
            parameters.Add(new SqlParameter("V_ID", "10"));
            var sql = JSqlBuilder.Instance.Build("PROC_TB_TODO_VIEW.js", parameters.ToArray());
            Console.WriteLine(sql);
        }
    }
}
