namespace SqlQuerySample.Sql;

public class QueryPocoJoinClause
{
    public string Key { get; set; }
    public string JoinType { get; set; }
    public string JoinTableName { get; set; }
    
    public List<QueryPocoWhereClause> JoinWhereItems { get; set; }
}

