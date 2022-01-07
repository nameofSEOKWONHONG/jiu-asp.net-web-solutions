using Chloe.DbExpressions;

namespace Chloe.Query
{
    public class JoinQueryResult
    {
        public IObjectModel ResultModel { get; set; }
        public DbJoinTableExpression JoinTable { get; set; }
    }
}
