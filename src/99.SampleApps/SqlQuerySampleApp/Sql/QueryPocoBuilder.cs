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

    public (string sql, List<object> bindingItems) Build()
    {
        var query = this.GenerateQuery();
        var compiled = _compiler.Compile(query);
        return (compiled.Sql, compiled.Bindings);
    }

    public Query Query => this.GenerateQuery();
    
    private Query GenerateQuery()
    {
        var query = _query.From(_queryPoco.FromTable);
        
        query.Select(_queryPoco.Selectors.ToArray());
        
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
            // join = join.On(item.First, item.Second, item.Op);
            // item.OnItems.xForEach(onItem =>
            // {
            //     join = join.On(onItem.First, onItem.Second.xValue<string>(), onItem.Op);
            // });
            
            item.JoinWhereItems.xForEach(joinItem =>
            {
                join = join.On(joinItem.First, joinItem.Second, joinItem.Op);
            });
            
            query.Join(item.JoinTableName, j => join);
        });

        _queryPoco.QueryPocoClauses.xForEach(item =>
        {
            query.Where(item.First, item.Op, item.SecondValue);
        });

        return query;
    }
}