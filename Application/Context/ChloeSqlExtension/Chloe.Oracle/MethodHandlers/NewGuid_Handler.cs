using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.Oracle.MethodHandlers
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
            //返回的是一个长度为 16 的 byte[]
            generator.SqlBuilder.Append("SYS_GUID()");
        }
    }
}
