using System.Data;
using eXtensionSharp;
using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace SqlQuerySample.Sql;

public class QueryPocoBuilder 
{
    private readonly SqlServerCompiler _compiler;
    private readonly IDbConnection _connection;
    private readonly QueryFactory _queryBuilder;
    private readonly Query _query;
    private readonly QueryPoco _queryPoco;
    
    public QueryPocoBuilder(QueryPoco queryPoco)
    {
        _compiler = new SqlServerCompiler();
        _connection = new SqlConnection("Data Source=Demo.db");
        _queryBuilder = new QueryFactory(_connection, _compiler);
        _query = new();
        _queryPoco = queryPoco;
    }

    public string Build()
    {
        var self = _query.From(_queryPoco.FromTable);
        _queryPoco.QueryPocoJoins.xForEach(item =>
        {
            var join = new Join().From(item.JoinTableName);
            if (item.JoinType == "inner join")
            {
                join = join.AsInner();
            }
            else if (item.JoinType == "left join")
            {
                join = join.AsLeft();
            }
            else if (item.JoinType == "right join")
            {
                join = join.AsRight();
            }
            join = join.On(item.First, item.Second, item.Op);
            item.OnItems.xForEach(onItem =>
            {
                join = join.On(onItem.First, onItem.Second.xValue<string>(), onItem.Op);
            });
            self.Join(item.JoinTableName, j => join);
        });

        _queryPoco.QueryPocoClauses.xForEach(item =>
        {
            self.Where(item.First, item.Op, item.Second);
        });

        var compiled = _compiler.Compile(self);
        return compiled.ToString();
    }
}