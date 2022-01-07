using Chloe.DbExpressions;
using System.Reflection;

namespace Chloe.Core.Visitors
{
    public class JoinConditionExpressionTransformer : DbExpressionVisitor
    {
        public static readonly MethodInfo MethodInfo_Sql_Equals = typeof(Sql).GetMethods().Where(a => a.Name == "Equals" && a.IsStatic && a.IsGenericMethod).First();
        public static readonly MethodInfo MethodInfo_Sql_NotEquals = typeof(Sql).GetMethod("NotEquals");

        static readonly JoinConditionExpressionTransformer _joinConditionExpressionParser = new JoinConditionExpressionTransformer();

        public static DbExpression Transform(DbExpression exp)
        {
            return exp.Accept(_joinConditionExpressionParser);
        }
        public override DbExpression Visit(DbEqualExpression exp)
        {
            /*
             * join 的条件不考虑 null 问题
             */

            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            MethodInfo method_Sql_Equals = MethodInfo_Sql_Equals.MakeGenericMethod(left.Type);

            /* Sql.Equals(left, right) */
            DbMethodCallExpression left_equals_right = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left.Accept(this), right.Accept(this) });

            return left_equals_right;
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            /*
             * join 的条件不考虑 null 问题
             */

            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            MethodInfo method_Sql_NotEquals = MethodInfo_Sql_NotEquals.MakeGenericMethod(left.Type);

            /* Sql.NotEquals(left, right) */
            DbMethodCallExpression left_not_equals_right = DbExpression.MethodCall(null, method_Sql_NotEquals, new List<DbExpression>(2) { left.Accept(this), right.Accept(this) });

            return left_not_equals_right;
        }
    }
}
