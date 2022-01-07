using Chloe.DbExpressions;
using Chloe.RDBMS;
using Chloe.Reflection;

namespace Chloe.SqlServer.Odbc
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
        static string GenRowNumberName(List<DbColumnSegment> columns)
        {
            int ROW_NUMBER_INDEX = 1;
            string row_numberName = "ROW_NUMBER_0";
            while (columns.Any(a => string.Equals(a.Alias, row_numberName, StringComparison.OrdinalIgnoreCase)))
            {
                row_numberName = "ROW_NUMBER_" + ROW_NUMBER_INDEX.ToString();
                ROW_NUMBER_INDEX++;
            }

            return row_numberName;
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


        static DbExpression EnsureDbExpressionReturnCSharpBoolean(DbExpression exp)
        {
            return DbValueExpressionTransformer.Transform(exp);
        }
        public static DbCaseWhenExpression ConstructReturnCSharpBooleanCaseWhenExpression(DbExpression exp)
        {
            // case when 1>0 then 1 when not (1>0) then 0 else Null end
            DbCaseWhenExpression.WhenThenExpressionPair whenThenPair = new DbCaseWhenExpression.WhenThenExpressionPair(exp, DbConstantExpression.True);
            DbCaseWhenExpression.WhenThenExpressionPair whenThenPair1 = new DbCaseWhenExpression.WhenThenExpressionPair(DbExpression.Not(exp), DbConstantExpression.False);
            List<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps = new List<DbCaseWhenExpression.WhenThenExpressionPair>(2);
            whenThenExps.Add(whenThenPair);
            whenThenExps.Add(whenThenPair1);
            DbCaseWhenExpression caseWhenExpression = DbExpression.CaseWhen(whenThenExps, DbConstantExpression.Null, PublicConstants.TypeOfBoolean);

            return caseWhenExpression;
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

        static bool TryGetCastTargetDbTypeString(Type sourceType, Type targetType, out string dbTypeString, bool throwNotSupportedException = true)
        {
            dbTypeString = null;

            sourceType = ReflectionExtension.GetUnderlyingType(sourceType);
            targetType = ReflectionExtension.GetUnderlyingType(targetType);

            if (sourceType == targetType)
                return false;

            if (targetType == PublicConstants.TypeOfDecimal)
            {
                //Casting to Decimal is not supported when missing the precision and scale information.I have no idea to deal with this case now.
                if (sourceType != PublicConstants.TypeOfInt16 && sourceType != PublicConstants.TypeOfInt32 && sourceType != PublicConstants.TypeOfInt64 && sourceType != PublicConstants.TypeOfByte)
                {
                    if (throwNotSupportedException)
                        throw new NotSupportedException(AppendNotSupportedCastErrorMsg(sourceType, targetType));
                    else
                        return false;
                }
            }

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
            generator.SqlBuilder.Append("DATEADD(");
            generator.SqlBuilder.Append(interval);
            generator.SqlBuilder.Append(",");
            exp.Arguments[0].Accept(generator);
            generator.SqlBuilder.Append(",");
            exp.Object.Accept(generator);
            generator.SqlBuilder.Append(")");
        }
        public static void DbFunction_DATEPART(SqlGeneratorBase generator, string interval, DbExpression exp)
        {
            generator.SqlBuilder.Append("DATEPART(");
            generator.SqlBuilder.Append(interval);
            generator.SqlBuilder.Append(",");
            exp.Accept(generator);
            generator.SqlBuilder.Append(")");
        }
        public static void DbFunction_DATEDIFF(SqlGeneratorBase generator, string interval, DbExpression startDateTimeExp, DbExpression endDateTimeExp)
        {
            generator.SqlBuilder.Append("DATEDIFF(");
            generator.SqlBuilder.Append(interval);
            generator.SqlBuilder.Append(",");
            startDateTimeExp.Accept(generator);
            generator.SqlBuilder.Append(",");
            endDateTimeExp.Accept(generator);
            generator.SqlBuilder.Append(")");
        }

        #region AggregateFunction
        public static void Aggregate_Count(SqlGeneratorBase generator)
        {
            generator.SqlBuilder.Append("COUNT(1)");
        }
        public static void Aggregate_LongCount(SqlGeneratorBase generator)
        {
            generator.SqlBuilder.Append("COUNT_BIG(1)");
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
                generator.SqlBuilder.Append("ISNULL(");
                AppendAggregateFunction(generator, exp, retType, "SUM", true);
                generator.SqlBuilder.Append(",");
                generator.SqlBuilder.Append("0");
                generator.SqlBuilder.Append(")");
            }
        }
        public static void Aggregate_Average(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            string targetDbType = null;

            Type underlyingType = ReflectionExtension.GetUnderlyingType(retType);
            if (underlyingType != exp.Type.GetUnderlyingType())
            {
                CastTypeMap.TryGetValue(underlyingType, out targetDbType);
            }

            generator.SqlBuilder.Append("AVG", "(");
            if (string.IsNullOrEmpty(targetDbType))
            {
                exp.Accept(generator);
            }
            else
            {
                generator.SqlBuilder.Append("CAST(");
                exp.Accept(generator);
                generator.SqlBuilder.Append(" AS ", targetDbType, ")");
            }

            generator.SqlBuilder.Append(")");
        }

        static void AppendAggregateFunction(SqlGeneratorBase generator, DbExpression exp, Type retType, string functionName, bool withCast)
        {
            string targetDbType = null;
            if (withCast == true)
            {
                Type underlyingType = ReflectionExtension.GetUnderlyingType(retType);
                if (underlyingType != PublicConstants.TypeOfDecimal/* We don't know the precision and scale,so,we can not cast exp to decimal,otherwise maybe cause problems. */ && CastTypeMap.TryGetValue(underlyingType, out targetDbType))
                {
                    generator.SqlBuilder.Append("CAST(");
                }
            }

            generator.SqlBuilder.Append(functionName, "(");
            exp.Accept(generator);
            generator.SqlBuilder.Append(")");

            if (targetDbType != null)
            {
                generator.SqlBuilder.Append(" AS ", targetDbType, ")");
            }
        }
        #endregion

    }
}
