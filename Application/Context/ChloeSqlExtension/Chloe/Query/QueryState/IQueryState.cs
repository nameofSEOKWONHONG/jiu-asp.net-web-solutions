using Chloe.Query.Mapping;
using Chloe.Query.QueryExpressions;
using Chloe.Utility;
using System.Linq.Expressions;

namespace Chloe.Query.QueryState
{
    interface IQueryState
    {
        MappingData GenerateMappingData();

        QueryModel ToFromQueryModel();
        JoinQueryResult ToJoinQueryResult(JoinType joinType, LambdaExpression conditionExpression, ScopeParameterDictionary scopeParameters, StringSet scopeTables, Func<string, string> tableAliasGenerator);

        IQueryState Accept(WhereExpression exp);
        IQueryState Accept(OrderExpression exp);
        IQueryState Accept(SelectExpression exp);
        IQueryState Accept(SkipExpression exp);
        IQueryState Accept(TakeExpression exp);
        IQueryState Accept(AggregateQueryExpression exp);
        IQueryState Accept(GroupingQueryExpression exp);
        IQueryState Accept(DistinctExpression exp);
        IQueryState Accept(IncludeExpression exp);
        IQueryState Accept(IgnoreAllFiltersExpression exp);
    }
}
