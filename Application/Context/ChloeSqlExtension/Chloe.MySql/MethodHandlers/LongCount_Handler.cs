using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.MySql.MethodHandlers
{
    class LongCount_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfSql)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            SqlGenerator.Aggregate_LongCount(generator);
        }
    }
}
