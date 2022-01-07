using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.PostgreSQL.MethodHandlers
{
    class AddHours_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfDateTime)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            List<DbExpression> arguments = new List<DbExpression>(exp.Arguments.Count);
            arguments.Add(new DbConvertExpression(typeof(int), exp.Arguments[0]));
            DbMethodCallExpression e = new DbMethodCallExpression(exp.Object, exp.Method, arguments);

            SqlGenerator.DbFunction_DATEADD(generator, "hours", e);
        }
    }
}
