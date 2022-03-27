using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities;
using Humanizer;
using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Application.Sql;

public class QueryPoco
{
    private SqlServerCompiler _compiler;
    private IDbConnection _connection;
    private QueryFactory _queryBuilder;
    private Query _query;
    public QueryPoco()
    {
        _compiler = new SqlServerCompiler();
        _connection = new SqlConnection("Data Source=Demo.db");
        _queryBuilder = new QueryFactory(_connection, _compiler);
    }

    public QueryPoco From<TTable>()
    {
        _query = _queryBuilder.Query(typeof(TTable).Name);
        return this;
    }

    public QueryPoco Join<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression)
    {
        var taAlias = expression.Parameters.First().Name;
        var tbAlias = expression.Parameters.Last().Name;
        dynamic body = expression.Body;
        var memberlist = new List<string>();
        foreach (var arg in body.Arguments) memberlist.Add($"{arg.Expression.Name}.{arg.Member.Name}");
        _query.Join(typeof(TTableB).Name, "", "", "", "");
        return this;
    }
}

public class Sample
{
    public void Run()
    {
        var q = new QueryPoco();
        q.From<TB_USER>().Join<TB_USER, TB_ROLE>((m, j) => m.ROLE.ID == j.ID);
    }
}

