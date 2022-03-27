namespace SqlQuerySample.Sql;

public class QueryPocoJoin
{
    public string Key { get; set; }
    public string JoinType { get; set; }
    public string JoinTableName { get; set; }
    public string First { get; set; }
    public string Second { get; set; }
    public string Op { get; set; }
    public List<QueryPocoClause> OnItems { get; set; }
}