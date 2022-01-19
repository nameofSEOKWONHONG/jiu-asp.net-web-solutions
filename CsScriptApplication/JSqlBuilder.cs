using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eXtensionSharp;
using Microsoft.ClearScript.V8;

namespace CsScriptApplication;

public class JSqlBuilder
{
    private static Lazy<JSqlBuilder> _instance = new Lazy<JSqlBuilder>(() => new JSqlBuilder());
    public static JSqlBuilder Instance => _instance.Value;

    private readonly Dictionary<string, ValueTuple<string, string>> _jsqlItems =
        new Dictionary<string, ValueTuple<string, string>>();
    
    private JSqlBuilder()
    {
        
    }

    public string Build(string fileName, IDataParameter[] parameters)
    {
        var exists = _jsqlItems.FirstOrDefault(m => m.Key == fileName);
        if (exists.Key.xIsEmpty())
        {
            var jsql = new JSql(fileName, parameters);
            var sql = jsql.ToString();
            var hash = sql.xToHash();
            _jsqlItems.Add(fileName, (sql, hash));
            return sql;
        }

        return exists.Value.Item2;
    }
}

public class JSql
{
    private readonly string _fileName;
    private readonly IDataParameter[] _parameters;
    
    public JSql(string fileName, IDataParameter[] parameters = null)
    {
        _fileName = fileName;
        _parameters = parameters;
    }

    public override string ToString()
    {
        var jsql = this._fileName.xFileReadAllText();
        var sb = new XStringBuilder();
        this._parameters.xForEach(parameter =>
        {
            sb.AppendLine($"var {parameter.ParameterName} = '{parameter.Value}'; ");
        });
        sb.Release(out string variables);
        var resultSql = $"{variables} {jsql}";

        using var runtime = new V8Runtime();
        var engine = runtime.CreateScriptEngine();
        engine.Execute(resultSql);
        
        return ((string) engine.Script.sql).Trim();
    }
}