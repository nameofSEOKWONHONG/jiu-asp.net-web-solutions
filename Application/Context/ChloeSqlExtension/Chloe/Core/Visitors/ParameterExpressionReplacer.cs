using System.Linq.Expressions;

namespace Chloe.Core.Visitors
{
    public class ParameterExpressionReplacer : ExpressionVisitor
    {
        ParameterExpression _replaceWith;

        ParameterExpressionReplacer(ParameterExpression replaceWith)
        {
            this._replaceWith = replaceWith;
        }

        public static Expression Replace(Expression expression, ParameterExpression replaceWith)
        {
            return new ParameterExpressionReplacer(replaceWith).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this._replaceWith;
        }
    }

}
