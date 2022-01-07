using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.SqlServer.Odbc.MethodHandlers
{
    class DiffMonths_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfSql)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            SqlGenerator.DbFunction_DATEDIFF(generator, "MONTH", exp.Arguments[0], exp.Arguments[1]);
        }
    }
}
