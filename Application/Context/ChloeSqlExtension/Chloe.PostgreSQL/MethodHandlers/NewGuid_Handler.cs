using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.PostgreSQL.MethodHandlers
{
    class NewGuid_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method != PublicConstants.MethodInfo_Guid_NewGuid)
                return false;

            return false;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            throw UtilExceptions.NotSupportedMethod(exp.Method);
        }
    }
}
