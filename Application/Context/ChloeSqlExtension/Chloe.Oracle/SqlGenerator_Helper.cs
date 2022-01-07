using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using Chloe.RDBMS;
using Chloe.Reflection;

namespace Chloe.Oracle
{
    partial class SqlGenerator : SqlGeneratorBase
    {
        void LeftBracket()
        {
            this.SqlBuilder.Append("(");
        }
        void RightBracket()
        {
            this.SqlBuilder.Append(")");
        }

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
            /*
             * Just support hour/minute/second
             * systimestamp + numtodsinterval(1,'HOUR')
             * sysdate + numtodsinterval(50,'MINUTE')
             * sysdate + numtodsinterval(45,'SECOND')
             */
            generator.SqlBuilder.Append("(");
            exp.Object.Accept(generator);
            generator.SqlBuilder.Append(" + ");
            generator.SqlBuilder.Append("NUMTODSINTERVAL(");
            exp.Arguments[0].Accept(generator);
            generator.SqlBuilder.Append(",'");
            generator.SqlBuilder.Append(interval);
            generator.SqlBuilder.Append("')");
            generator.SqlBuilder.Append(")");
        }
        public static void DbFunction_DATEPART(SqlGeneratorBase generator, string interval, DbExpression exp, bool castToTimestamp = false)
        {
            /* cast(to_char(sysdate,'yyyy') as number) */
            generator.SqlBuilder.Append("CAST(TO_CHAR(");
            if (castToTimestamp)
            {
                (generator as SqlGenerator).BuildCastState(exp, "TIMESTAMP");
            }
            else
                exp.Accept(generator);
            generator.SqlBuilder.Append(",'");
            generator.SqlBuilder.Append(interval);
            generator.SqlBuilder.Append("') AS NUMBER)");
        }
        public static void DbFunction_DATEDIFF(SqlGeneratorBase generator, string interval, DbExpression startDateTimeExp, DbExpression endDateTimeExp)
        {
            throw new NotSupportedException("DATEDIFF is not supported.");
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
                AppendAggregateFunction(generator, exp, retType, "SUM", false);
            }
            else
            {
                generator.SqlBuilder.Append("NVL(");
                AppendAggregateFunction(generator, exp, retType, "SUM", false);
                generator.SqlBuilder.Append(",");
                generator.SqlBuilder.Append("0");
                generator.SqlBuilder.Append(")");
            }
        }
        public static void Aggregate_Average(SqlGeneratorBase generator, DbExpression exp, Type retType)
        {
            generator.SqlBuilder.Append("TRUNC", "(");
            AppendAggregateFunction(generator, exp, retType, "AVG", false);
            generator.SqlBuilder.Append(",7)");
        }

        static void AppendAggregateFunction(SqlGeneratorBase generator, DbExpression exp, Type retType, string functionName, bool withCast)
        {
            string dbTypeString = null;
            if (withCast == true)
            {
                Type underlyingType = ReflectionExtension.GetUnderlyingType(retType);
                if (CastTypeMap.TryGetValue(underlyingType, out dbTypeString))
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


        static void CopyColumnSegments(List<DbColumnSegment> sourceList, List<DbColumnSegment> destinationList, DbTable newTable)
        {
            for (int i = 0; i < sourceList.Count; i++)
            {
                DbColumnSegment newColumnSeg = CloneColumnSegment(sourceList[i], newTable);
                destinationList.Add(newColumnSeg);
            }
        }
        static DbColumnSegment CloneColumnSegment(DbColumnSegment rawColumnSeg, DbTable newBelongTable)
        {
            DbColumnAccessExpression columnAccessExp = new DbColumnAccessExpression(newBelongTable, DbColumn.MakeColumn(rawColumnSeg.Body, rawColumnSeg.Alias));
            DbColumnSegment newColumnSeg = new DbColumnSegment(columnAccessExp, rawColumnSeg.Alias);

            return newColumnSeg;
        }
        static void AppendLimitCondition(DbSqlQueryExpression sqlQuery, int limitCount)
        {
            DbLessThanExpression lessThanExp = DbExpression.LessThan(OracleSemantics.DbMemberExpression_ROWNUM, DbExpression.Constant(limitCount + 1));

            DbExpression condition = lessThanExp;
            if (sqlQuery.Condition != null)
                condition = DbExpression.And(sqlQuery.Condition, condition);

            sqlQuery.Condition = condition;
        }
        static DbSqlQueryExpression WrapSqlQuery(DbSqlQueryExpression sqlQuery, DbTable table, List<DbColumnSegment> columnSegments = null)
        {
            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);

            DbSqlQueryExpression newSqlQuery = new DbSqlQueryExpression();

            DbTableSegment tableSeg = new DbTableSegment(subQuery, table.Name, LockType.Unspecified);
            DbFromTableExpression fromTableExp = new DbFromTableExpression(tableSeg);

            newSqlQuery.Table = fromTableExp;

            CopyColumnSegments(columnSegments ?? subQuery.SqlQuery.ColumnSegments, newSqlQuery.ColumnSegments, table);

            return newSqlQuery;
        }
        static DbSqlQueryExpression CloneWithoutLimitInfo(DbSqlQueryExpression sqlQuery, string wraperTableName = "T")
        {
            DbSqlQueryExpression newSqlQuery = new DbSqlQueryExpression();
            newSqlQuery.Table = sqlQuery.Table;
            newSqlQuery.ColumnSegments.AddRange(sqlQuery.ColumnSegments);
            newSqlQuery.Condition = sqlQuery.Condition;

            newSqlQuery.GroupSegments.AddRange(sqlQuery.GroupSegments);
            newSqlQuery.HavingCondition = sqlQuery.HavingCondition;

            newSqlQuery.Orderings.AddRange(sqlQuery.Orderings);

            if (sqlQuery.Orderings.Count > 0 || sqlQuery.GroupSegments.Count > 0)
            {
                newSqlQuery = WrapSqlQuery(newSqlQuery, new DbTable(wraperTableName));
            }

            return newSqlQuery;
        }
    }
}
