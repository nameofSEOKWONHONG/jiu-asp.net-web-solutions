using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Utility;
using System.Linq.Expressions;

namespace Chloe.Query.Visitors
{
    class FilterPredicateParser : ExpressionVisitor<DbExpression>
    {
        public static DbExpression Parse(LambdaExpression filterPredicate, ScopeParameterDictionary scopeParameters, StringSet scopeTables)
        {
            return GeneralExpressionParser.Parse(BooleanResultExpressionTransformer.Transform(filterPredicate), scopeParameters, scopeTables);
        }
    }
}
