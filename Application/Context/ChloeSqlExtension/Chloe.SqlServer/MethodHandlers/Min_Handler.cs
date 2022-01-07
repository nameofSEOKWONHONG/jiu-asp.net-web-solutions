using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.SqlServer.MethodHandlers
{
    class Min_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfSql)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            SqlGenerator.Aggregate_Min(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
    }
}
