using Chloe.DbExpressions;
using Chloe.RDBMS;
using Chloe.InternalExtensions;

namespace Chloe.Oracle.MethodHandlers
{
    class NextValueForSequence_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            if (exp.Method.DeclaringType != PublicConstants.TypeOfSql)
                return false;

            return true;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            /* SELECT "SYSTEM"."USERS_AUTOID"."NEXTVAL" FROM "DUAL" */

            string sequenceName = (string)exp.Arguments[0].Evaluate();
            if (string.IsNullOrEmpty(sequenceName))
                throw new ArgumentException("The sequence name cannot be empty.");

            string sequenceSchema = (string)exp.Arguments[1].Evaluate();

            if (!string.IsNullOrEmpty(sequenceSchema))
            {
                (generator as SqlGenerator).QuoteName(sequenceSchema);
                generator.SqlBuilder.Append(".");
            }

            (generator as SqlGenerator).QuoteName(sequenceName);
            generator.SqlBuilder.Append(".nextval");
        }
    }
}
