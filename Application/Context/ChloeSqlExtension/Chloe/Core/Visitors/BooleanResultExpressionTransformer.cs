using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Chloe.Core.Visitors
{
    class BooleanResultExpressionTransformer : ExpressionVisitor<Expression>
    {
        static BooleanResultExpressionTransformer _transformer = new BooleanResultExpressionTransformer();

        static BooleanResultExpressionTransformer()
        {
            List<ExpressionType> booleanResultBinaryExpressionTypes = new List<ExpressionType>();
            booleanResultBinaryExpressionTypes.Add(ExpressionType.Equal);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.NotEqual);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.GreaterThan);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.GreaterThanOrEqual);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.LessThan);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.LessThanOrEqual);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.AndAlso);
            booleanResultBinaryExpressionTypes.Add(ExpressionType.OrElse);

            BooleanResultBinaryExpressionTypes = booleanResultBinaryExpressionTypes.AsReadOnly();
        }

        public static ReadOnlyCollection<ExpressionType> BooleanResultBinaryExpressionTypes { get; private set; }

        public static Expression Transform(Expression predicate)
        {
            return _transformer.Visit(predicate);
        }

        public override Expression Visit(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                default:
                    {
                        if (exp.Type != PublicConstants.TypeOfBoolean)
                            return exp;

                        if (!BooleanResultBinaryExpressionTypes.Contains(exp.NodeType))
                            exp = Expression.Equal(exp, UtilConstants.Constant_True);

                        return exp;
                    }
            }
        }

        protected override Expression VisitLambda(LambdaExpression exp)
        {
            if (!BooleanResultBinaryExpressionTypes.Contains(exp.Body.NodeType))
                exp = Expression.Lambda(Expression.Equal(exp.Body, UtilConstants.Constant_True), exp.Parameters.ToArray());

            return exp;
        }
    }
}
