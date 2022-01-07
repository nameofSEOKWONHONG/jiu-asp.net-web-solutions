using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.SqlServer.MethodHandlers
{
    class DiffMinutes_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfSql)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            SqlGenerator.DbFunction_DATEDIFF(generator, "MINUTE", exp.Arguments[0], exp.Arguments[1]);
        }
    }
}
