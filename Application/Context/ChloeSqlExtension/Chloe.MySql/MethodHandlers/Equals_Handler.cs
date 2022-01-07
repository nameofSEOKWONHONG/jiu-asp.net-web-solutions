using Chloe.DbExpressions;
using Chloe.RDBMS;
using Chloe.InternalExtensions;
using System.Reflection;

namespace Chloe.MySql.MethodHandlers
{
    class Equals_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            MethodInfo method = exp.Method;

            if (method.DeclaringType == PublicConstants.TypeOfSql)
            {
                return true;
            }

            if (method.ReturnType != PublicConstants.TypeOfBoolean || method.IsStatic || method.GetParameters().Length != 1)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            MethodInfo method = exp.Method;
            if (method.DeclaringType == PublicConstants.TypeOfSql)
            {
                Method_Sql_Equals(exp, generator);
                return;
            }

            DbExpression right = exp.Arguments[0];
            if (right.Type != exp.Object.Type)
            {
                right = DbExpression.Convert(right, exp.Object.Type);
            }

            DbExpression.Equal(exp.Object, right).Accept(generator);
        }

        static void Method_Sql_Equals(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            DbExpression left = exp.Arguments[0];
            DbExpression right = exp.Arguments[1];

            left = DbExpressionExtension.StripInvalidConvert(left);
            right = DbExpressionExtension.StripInvalidConvert(right);

            //明确 left right 其中一边一定为 null
            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(right))
            {
                left.Accept(generator);
                generator.SqlBuilder.Append(" IS NULL");
                return;
            }

            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(left))
            {
                right.Accept(generator);
                generator.SqlBuilder.Append(" IS NULL");
                return;
            }

            SqlGenerator.AmendDbInfo(left, right);

            left.Accept(generator);
            generator.SqlBuilder.Append(" = ");
            right.Accept(generator);
        }
    }
}
