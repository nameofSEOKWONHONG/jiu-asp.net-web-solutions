using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.SQLite.MethodHandlers
{
    class AddDays_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfDateTime)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            SqlGenerator.DbFunction_DATEADD(generator, "days", exp);
        }
    }
}
