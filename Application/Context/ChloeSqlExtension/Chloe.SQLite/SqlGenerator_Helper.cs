using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using Chloe.RDBMS;
using Chloe.Reflection;

namespace Chloe.SQLite
{
    partial class SqlGenerator : SqlGeneratorBase
    {
        static string GenParameterName(int ordinal)
        {
            if (ordinal < CacheParameterNames.Count)
            {
                return CacheParameterNames[ordinal];
            }

            return UtilConstants.ParameterNamePrefix + ordinal.ToString();
        }
        public static void AmendDbInfo(DbExpression exp1, DbExpression exp2)
        {
            DbColumnAccessExpression datumPointExp = null;
            DbParameterExpression expToAmend = null;

            DbExpression e = Trim_Nullable_Value(exp1);
            if (e.NodeType == DbExpressionType.ColumnAccess && exp2.NodeType == DbExpressionType.Parameter)
            {
                datumPointExp = (DbColumnAccessExpression)e;
                expToAmend = (DbParameterExpression)exp2;
            }
            else if ((e = Trim_Nullable_Value(exp2)).NodeType == DbExpressionType.ColumnAccess && exp1.NodeType == DbExpressionType.Parameter)
            {
                datumPointExp = (DbColumnAccessExpression)e;
                expToAmend = (DbParameterExpression)exp1;
            }
            else
                return;

            if (datumPointExp.Column.DbType != null)
            {
                if (expToAmend.DbType == null)
                    expToAmend.DbType = datumPointExp.Column.DbType;
            }
        }
        public static void AmendDbInfo(DbColumn column, DbExpression exp)
        {
            if (column.DbType == null || exp.NodeType != DbExpressionType.Parameter)
                return;

            DbParameterExpression expToAmend = (DbParameterExpression)exp;

            if (expToAmend.DbType == null)
                expToAmend.DbType = column.DbType;
        }
        static DbExpression Trim_Nullable_Value(DbExpression exp)
        {
            DbMemberExpression memberExp = exp as DbMemberExpression;
            if (memberExp == null)
                return exp;

            if (memberExp.Member.Name == "Value" && ReflectionExtension.IsNullable(memberExp.Expression.Type))
                return memberExp.Expression;

            return exp;
        }


        static Stack<DbExpression> GatherBinaryExpressionOperand(DbBinaryExpression exp)
        {
            DbExpressionType nodeType = exp.NodeType;

            Stack<DbExpression> items = new Stack<DbExpression>();
            items.Push(exp.Right);

            DbExpression left = exp.Left;
            while (left.NodeType == nodeType)
            {
                exp = (DbBinaryExpression)left;
                items.Push(exp.Right);
                left = exp.Left;
            }

            items.Push(left);
            return items;
        }

        static void EnsureTrimCharArgumentIsSpaces(DbExpression exp)
        {
            var m = exp as DbMemberExpression;
            if (m == null)
                throw new NotSupportedException();

            DbParameterExpression p;
            if (!DbExpressionExtension.TryConvertToParameterExpression(m, out p))
            {
                throw new NotSupportedException();
            }

            var arg = p.Value;

            if (arg == null)
                throw new NotSupportedException();

            var chars = arg as char[];
            if (chars.Length != 1 || chars[0] != ' ')
            {
                throw new NotSupportedException();
            }
        }
        static bool TryGetCastTargetDbTypeString(Type sourceType, Type targetType, out string dbTypeString, bool throwNotSupportedException = true)
        {
            dbTypeString = null;

            sourceType = ReflectionExtension.GetUnderlyingType(sourceType);
            targetType = ReflectionExtension.GetUnderlyingType(targetType);

            if (sourceType == targetType)
                return false;

            if (CastTypeMap.TryGetValue(targetType, out dbTypeString))
            {
                return true;
            }

            if (throwNotSupportedException)
                throw new NotSupportedException(AppendNotSupportedCastErrorMsg(sourceType, targetType));
            else
                return false;
        }
        static string AppendNotSupportedCastErrorMsg(Type sourceType, Type targetType)
        {
            return string.Format("Does not support the type '{0}' converted to type '{1}'.", sourceType.FullName, targetType.FullName);
        }

        public static void DbFunction_DATEADD(SqlGeneratorBase generator, string interval, DbMethodCallExpression exp)
        {
            /* DATETIME(@P_0,'+' || 1 || ' years') */

            generator.SqlBuilder.Append("DATETIME(");
            exp.Object.Accept(generator);
            generator.SqlBuilder.Append(",'+' || ");
            exp.Arguments[0].Accept(generator);
            generator.SqlBuilder.Append(" || ' ", interval, "'");
            generator.SqlBuilder.Append(")");
        }
        public static void DbFunction_DATEPART(SqlGeneratorBase generator, string interval, DbExpression exp)
        {
            /* CAST(STRFTIME('%M','2016-08-06 09:01:24') AS INTEGER) */
            generator.SqlBuilder.Append("CAST(");
            generator.SqlBuilder.Append("STRFTIME('%", interval, "',");
            exp.Accept(generator);
            generator.SqlBuilder.Append(")");
            generator.SqlBuilder.Append(" AS INTEGER)");
        }

        static void Append_JULIANDAY(SqlGeneratorBase generator, DbExpression startDateTimeExp, DbExpression endDateTimeExp)
        {
            /* (JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) */

            generator.SqlBuilder.Append("(");

            generator.SqlBuilder.Append("JULIANDAY(");
            endDateTimeExp.Accept(generator);
            generator.SqlBuilder.Append(")");

            generator.SqlBuilder.Append(" - ");

            generator.SqlBuilder.Append("JULIANDAY(");
            startDateTimeExp.Accept(generator);
            generator.SqlBuilder.Append(")");

            generator.SqlBuilder.Append(")");
        }
        public static void Append_DiffYears(SqlGeneratorBase generator, DbExpression startDateTimeExp, DbExpression endDateTimeExp)
        {
            /* (CAST(STRFTIME('%Y',endDateTimeExp) as INTEGER) - CAST(STRFTIME('%Y',startDateTimeExp) as INTEGER)) */

            generator.SqlBuilder.Append("(");
            DbFunction_DATEPART(generator, "Y", endDateTimeExp);
            generator.SqlBuilder.Append(" - ");
            DbFunction_DATEPART(generator, "Y", startDateTimeExp);
            generator.SqlBuilder.Append(")");
        }
        public static void Append_DateDiff(SqlGeneratorBase generator, DbExpression startDateTimeExp, DbExpression endDateTimeExp, int? multiplier)
        {
            /* CAST((JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) AS INTEGER) */
            /* OR */
            /* CAST((JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) * multiplier AS INTEGER) */

            generator.SqlBuilder.Append("CAST(");

            Append_JULIANDAY(generator, startDateTimeExp, endDateTimeExp);
            if (multiplier != null)
                generator.SqlBuilder.Append(" * ", multiplier.Value.ToString());

            generator.SqlBuilder.Append(" AS INTEGER)");
        }

        #region AggregateFunction
        public static void Aggregate_Count(SqlGeneratorBase generator)
        {
            generator.SqlBuilder.Append("COUNT(1)");
        }
        public static void Aggregate_LongCount(SqlGeneratorBase generator)
        {
            generator.SqlBuilder.Append("COUNT(1)");
        }
        public static void Aggregate_Max(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "MAX", false);
        }
        public static void Aggregate_Min(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "MIN", false);
        }
        public static void Aggregate_Sum(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            if (retType.IsNullable())
            {
                AppendAggregateFunction(generator, exp, retType, "SUM", true);
            }
            else
            {
                generator.SqlBuilder.Append("IFNULL(");
                AppendAggregateFunction(generator, exp, retType, "SUM", true);
                generator.SqlBuilder.Append(",");
                generator.SqlBuilder.Append("0");
                generator.SqlBuilder.Append(")");
            }
        }
        public static void Aggregate_Average(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "AVG", true);
        }

        static void AppendAggregateFunction(SqlGeneratorBase generator, DbExpression exp, Type retType, string functionName, bool withCast)
        {
            string dbTypeString = null;
            if (withCast == true)
            {
                Type underlyingType = ReflectionExtension.GetUnderlyingType(retType);
                if (underlyingType != PublicConstants.TypeOfDecimal/* We don't know the precision and scale,so,we can not cast exp to decimal,otherwise maybe cause problems. */ && CastTypeMap.TryGetValue(underlyingType, out dbTypeString))
                {
                    generator.SqlBuilder.Append("CAST(");
                }
            }

            generator.SqlBuilder.Append(functionName, "(");
            exp.Accept(generator);
            generator.SqlBuilder.Append(")");

            if (dbTypeString != null)
            {
                generator.SqlBuilder.Append(" AS ", dbTypeString, ")");
            }
        }
        #endregion

    }
}
