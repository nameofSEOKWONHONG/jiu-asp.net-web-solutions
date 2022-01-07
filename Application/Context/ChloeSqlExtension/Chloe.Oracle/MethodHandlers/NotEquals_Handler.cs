using Chloe.DbExpressions;
using Chloe.RDBMS;
using Chloe.InternalExtensions;
using System.Reflection;

namespace Chloe.Oracle.MethodHandlers
{
    class NotEquals_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            MethodInfo method = exp.Method;
            if (method.DeclaringType != PublicConstants.TypeOfSql)
            {
                return false;
            }

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            DbExpression left = exp.Arguments[0];
            DbExpression right = exp.Arguments[1];

            left = DbExpressionExtension.StripInvalidConvert(left);
            right = DbExpressionExtension.StripInvalidConvert(right);

            //明确 left right 其中一边一定为 null
            if (DbExpressionHelper.AffirmExpressionRetValueIsNullOrEmpty(right))
            {
                left.Accept(generator);
                generator.SqlBuilder.Append(" IS NOT NULL");
                return;
            }

            if (DbExpressionHelper.AffirmExpressionRetValueIsNullOrEmpty(left))
            {
                right.Accept(generator);
                generator.SqlBuilder.Append(" IS NOT NULL");
                return;
            }

            SqlGenerator.AmendDbInfo(left, right);

            left.Accept(generator);
            generator.SqlBuilder.Append(" <> ");
            right.Accept(generator);
        }
    }
}
