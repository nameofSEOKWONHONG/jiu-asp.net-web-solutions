using Chloe.DbExpressions;
using Chloe.RDBMS;

namespace Chloe.SQLite.MethodHandlers
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
            DbExpression startDateTimeExp = exp.Arguments[0];
            DbExpression endDateTimeExp = exp.Arguments[1];

            /*
             * This method will generating sql like following:
              (cast(STRFTIME('%Y','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%Y','2015-08-06 09:01:24') as INTEGER)) * 12  + (cast(STRFTIME('%m','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%m','2015-08-06 09:01:24') as INTEGER))
             */

            generator.SqlBuilder.Append("(");

            /* (cast(STRFTIME('%Y','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%Y','2015-08-06 09:01:24') as INTEGER)) * 12 */
            SqlGenerator.Append_DiffYears(generator, startDateTimeExp, endDateTimeExp);
            generator.SqlBuilder.Append(" * 12");

            generator.SqlBuilder.Append(" + ");

            /* (cast(STRFTIME('%m','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%m','2015-08-06 09:01:24') as INTEGER)) */
            generator.SqlBuilder.Append("(");
            SqlGenerator.DbFunction_DATEPART(generator, "m", endDateTimeExp);
            generator.SqlBuilder.Append(" - ");
            SqlGenerator.DbFunction_DATEPART(generator, "m", startDateTimeExp);
            generator.SqlBuilder.Append(")");

            generator.SqlBuilder.Append(")");
        }
    }
}
