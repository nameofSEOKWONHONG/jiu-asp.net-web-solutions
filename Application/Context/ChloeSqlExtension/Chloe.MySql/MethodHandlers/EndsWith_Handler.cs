using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.MySql.MethodHandlers
{
    class EndsWith_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method != PublicConstants.MethodInfo_String_EndsWith)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            exp.Object.Accept(generator);

            generator.SqlBuilder.Append(" LIKE ");
            generator.SqlBuilder.Append("CONCAT(");
            generator.SqlBuilder.Append("'%',");
            exp.Arguments.First().Accept(generator);
            generator.SqlBuilder.Append(")");
        }
    }

}
