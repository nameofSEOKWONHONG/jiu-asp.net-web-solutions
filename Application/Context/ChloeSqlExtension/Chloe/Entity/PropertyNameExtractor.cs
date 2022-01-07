using Chloe.Core.Visitors;
using System.Linq.Expressions;

namespace Chloe.Entity
{
    class PropertyNameExtractor : ExpressionVisitor<string>
    {
        static readonly PropertyNameExtractor _extractor = new PropertyNameExtractor();
        PropertyNameExtractor()
        {
        }
        public static string Extract(Expression exp)
        {
            return _extractor.Visit(exp);
        }
        public override string Visit(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Convert:
                    return this.VisitUnary_Convert((UnaryExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }
        protected override string VisitLambda(LambdaExpression exp)
        {
            return this.Visit(exp.Body);
        }
        protected override string VisitMemberAccess(MemberExpression exp)
        {
            return exp.Member.Name;
        }
        protected override string VisitUnary_Convert(UnaryExpression exp)
        {
            return this.Visit(exp.Operand);
        }
    }
}
