using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.MySql.MethodHandlers
{
    class Replace_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method != PublicConstants.MethodInfo_String_Replace)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            generator.SqlBuilder.Append("REPLACE(");
            exp.Object.Accept(generator);
            generator.SqlBuilder.Append(",");
            exp.Arguments[0].Accept(generator);
            generator.SqlBuilder.Append(",");
            exp.Arguments[1].Accept(generator);
            generator.SqlBuilder.Append(")");
        }
    }
}
