using Chloe.DbExpressions;
using Chloe.RDBMS;
using Chloe.InternalExtensions;

namespace Chloe.PostgreSQL.MethodHandlers
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
            /* select nextval('public.users_auto_id')  */

            string sequenceName = (string)exp.Arguments[0].Evaluate();
            if (string.IsNullOrEmpty(sequenceName))
                throw new ArgumentException("The sequence name cannot be empty.");

            string sequenceSchema = (string)exp.Arguments[1].Evaluate();

            generator.SqlBuilder.Append("nextval('");

            if (!string.IsNullOrEmpty(sequenceSchema))
            {
                generator.SqlBuilder.Append(sequenceSchema, ".");
            }

            generator.SqlBuilder.Append(sequenceName);
            generator.SqlBuilder.Append("')");
        }
    }
}
