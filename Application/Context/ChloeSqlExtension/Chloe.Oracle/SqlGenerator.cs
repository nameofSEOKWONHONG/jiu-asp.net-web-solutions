using Chloe.Annotations;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using Chloe.RDBMS;
using Chloe.Reflection;
using System.Data;
using System.Reflection;

namespace Chloe.Oracle
{
    partial class SqlGenerator : SqlGeneratorBase
    {
        static readonly object Boxed_1 = 1;
        static readonly object Boxed_0 = 0;

        DbParamCollection _parameters = new DbParamCollection();

        public static readonly Dictionary<string, IMethodHandler> MethodHandlers = GetMethodHandlers();
        static readonly Dictionary<string, Action<DbAggregateExpression, SqlGeneratorBase>> AggregateHandlers = InitAggregateHandlers();
        static readonly Dictionary<MethodInfo, Action<DbBinaryExpression, SqlGeneratorBase>> BinaryWithMethodHandlers = InitBinaryWithMethodHandlers();
        static readonly Dictionary<Type, string> CastTypeMap;
        public static readonly Dictionary<Type, Type> NumericTypes;
        static readonly List<string> CacheParameterNames;

        static SqlGenerator()
        {
            Dictionary<Type, string> castTypeMap = new Dictionary<Type, string>();
            //castTypeMap.Add(typeof(string), "NVARCHAR2"); // instead of using to_char(exp) 
            castTypeMap.Add(typeof(byte), "NUMBER(3,0)");
            castTypeMap.Add(typeof(Int16), "NUMBER(4,0)");
            castTypeMap.Add(typeof(int), "NUMBER(9,0)");
            castTypeMap.Add(typeof(long), "NUMBER(18,0)");
            castTypeMap.Add(typeof(float), "BINARY_FLOAT");
            castTypeMap.Add(typeof(double), "BINARY_DOUBLE");
            castTypeMap.Add(typeof(decimal), "NUMBER");
            castTypeMap.Add(typeof(bool), "NUMBER(9,0)");
            //castTypeMap.Add(typeof(DateTime), "DATE"); // instead of using TO_TIMESTAMP(exp) 
            //castTypeMap.Add(typeof(Guid), "BLOB");
            CastTypeMap = PublicHelper.Clone(castTypeMap);


            Dictionary<Type, Type> numericTypes = new Dictionary<Type, Type>();
            numericTypes.Add(typeof(byte), typeof(byte));
            numericTypes.Add(typeof(sbyte), typeof(sbyte));
            numericTypes.Add(typeof(short), typeof(short));
            numericTypes.Add(typeof(ushort), typeof(ushort));
            numericTypes.Add(typeof(int), typeof(int));
            numericTypes.Add(typeof(uint), typeof(uint));
            numericTypes.Add(typeof(long), typeof(long));
            numericTypes.Add(typeof(ulong), typeof(ulong));
            numericTypes.Add(typeof(float), typeof(float));
            numericTypes.Add(typeof(double), typeof(double));
            numericTypes.Add(typeof(decimal), typeof(decimal));
            NumericTypes = PublicHelper.Clone(numericTypes);


            int cacheParameterNameCount = 2 * 12;
            List<string> cacheParameterNames = new List<string>(cacheParameterNameCount);
            for (int i = 0; i < cacheParameterNameCount; i++)
            {
                string paramName = UtilConstants.ParameterNamePrefix + i.ToString();
                cacheParameterNames.Add(paramName);
            }
            CacheParameterNames = cacheParameterNames;
        }

        public List<DbParam> Parameters { get { return this._parameters.ToParameterList(); } }

        public static SqlGenerator CreateInstance()
        {
            return new SqlGenerator();
        }

        public override DbExpression Visit(DbEqualExpression exp)
        {
            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            left = DbExpressionExtension.StripInvalidConvert(left);
            right = DbExpressionExtension.StripInvalidConvert(right);

            MethodInfo method_Sql_Equals = PublicConstants.MethodInfo_Sql_Equals.MakeGenericMethod(left.Type);

            /* Sql.Equals(left, right) */
            DbMethodCallExpression left_equals_right = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left, right });

            if (right.NodeType == DbExpressionType.Parameter || right.NodeType == DbExpressionType.Constant || left.NodeType == DbExpressionType.Parameter || left.NodeType == DbExpressionType.Constant || right.NodeType == DbExpressionType.SubQuery || left.NodeType == DbExpressionType.SubQuery || !left.Type.CanNull() || !right.Type.CanNull())
            {
                /*
                 * a.Name == name --> a.Name == name
                 * a.Id == (select top 1 T.Id from T) --> a.Id == (select top 1 T.Id from T)
                 * 对于上述查询，我们不考虑 null
                 */

                left_equals_right.Accept(this);
                return exp;
            }


            /*
             * a.Name == a.XName --> a.Name == a.XName or (a.Name is null and a.XName is null)
             */

            /* Sql.Equals(left, null) */
            var left_is_null = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left, DbExpression.Constant(null, left.Type) });

            /* Sql.Equals(right, null) */
            var right_is_null = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { right, DbExpression.Constant(null, right.Type) });

            /* Sql.Equals(left, null) && Sql.Equals(right, null) */
            var left_is_null_and_right_is_null = DbExpression.And(left_is_null, right_is_null);

            /* Sql.Equals(left, right) || (Sql.Equals(left, null) && Sql.Equals(right, null)) */
            var left_equals_right_or_left_is_null_and_right_is_null = DbExpression.Or(left_equals_right, left_is_null_and_right_is_null);

            left_equals_right_or_left_is_null_and_right_is_null.Accept(this);

            return exp;
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            left = DbExpressionExtension.StripInvalidConvert(left);
            right = DbExpressionExtension.StripInvalidConvert(right);

            MethodInfo method_Sql_NotEquals = PublicConstants.MethodInfo_Sql_NotEquals.MakeGenericMethod(left.Type);

            /* Sql.NotEquals(left, right) */
            DbMethodCallExpression left_not_equals_right = DbExpression.MethodCall(null, method_Sql_NotEquals, new List<DbExpression>(2) { left, right });

            //明确 left right 其中一边一定为 null
            if (DbExpressionHelper.AffirmExpressionRetValueIsNullOrEmpty(right) || DbExpressionHelper.AffirmExpressionRetValueIsNullOrEmpty(left))
            {
                /*
                 * a.Name != null --> a.Name != null
                 */

                left_not_equals_right.Accept(this);
                return exp;
            }

            if (right.NodeType == DbExpressionType.SubQuery || left.NodeType == DbExpressionType.SubQuery)
            {
                /*
                 * a.Id != (select top 1 T.Id from T) --> a.Id <> (select top 1 T.Id from T)，对于这种查询，我们不考虑 null
                 */

                left_not_equals_right.Accept(this);
                return exp;
            }

            MethodInfo method_Sql_Equals = PublicConstants.MethodInfo_Sql_Equals.MakeGenericMethod(left.Type);

            if (left.NodeType == DbExpressionType.Parameter || left.NodeType == DbExpressionType.Constant)
            {
                var t = right;
                right = left;
                left = t;
            }
            if (right.NodeType == DbExpressionType.Parameter || right.NodeType == DbExpressionType.Constant)
            {
                /*
                 * 走到这说明 name 不可能为 null
                 * a.Name != name --> a.Name <> name or a.Name is null
                 */

                if (left.NodeType != DbExpressionType.Parameter && left.NodeType != DbExpressionType.Constant)
                {
                    /*
                     * a.Name != name --> a.Name <> name or a.Name is null
                     */

                    /* Sql.Equals(left, null) */
                    var left_is_null1 = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left, DbExpression.Constant(null, left.Type) });

                    /* Sql.NotEquals(left, right) || Sql.Equals(left, null) */
                    var left_not_equals_right_or_left_is_null = DbExpression.Or(left_not_equals_right, left_is_null1);
                    left_not_equals_right_or_left_is_null.Accept(this);
                }
                else
                {
                    /*
                     * name != name1 --> name <> name，其中 name 和 name1 都为变量且都不可能为 null
                     */

                    left_not_equals_right.Accept(this);
                }

                return exp;
            }


            /*
             * a.Name != a.XName --> a.Name <> a.XName or (a.Name is null and a.XName is not null) or (a.Name is not null and a.XName is null)
             * ## a.Name != a.XName 不能翻译成：not (a.Name == a.XName or (a.Name is null and a.XName is null))，因为数据库里的 not 有时候并非真正意义上的“取反”！
             * 当 a.Name 或者 a.XName 其中一个字段有为 NULL，另一个字段有值时，会查不出此条数据 ##
             */

            DbConstantExpression null_Constant = DbExpression.Constant(null, left.Type);

            /* Sql.Equals(left, null) */
            var left_is_null = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left, null_Constant });
            /* Sql.NotEquals(left, null) */
            var left_is_not_null = DbExpression.MethodCall(null, method_Sql_NotEquals, new List<DbExpression>(2) { left, null_Constant });

            /* Sql.Equals(right, null) */
            var right_is_null = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { right, null_Constant });
            /* Sql.NotEquals(right, null) */
            var right_is_not_null = DbExpression.MethodCall(null, method_Sql_NotEquals, new List<DbExpression>(2) { right, null_Constant });

            /* Sql.Equals(left, null) && Sql.NotEquals(right, null) */
            var left_is_null_and_right_is_not_null = DbExpression.And(left_is_null, right_is_not_null);

            /* Sql.NotEquals(left, null) && Sql.Equals(right, null) */
            var left_is_not_null_and_right_is_null = DbExpression.And(left_is_not_null, right_is_null);

            /* (Sql.Equals(left, null) && Sql.NotEquals(right, null)) || (Sql.NotEquals(left, null) && Sql.Equals(right, null)) */
            var left_is_null_and_right_is_not_null_or_left_is_not_null_and_right_is_null = DbExpression.Or(left_is_null_and_right_is_not_null, left_is_not_null_and_right_is_null);

            /* Sql.NotEquals(left, right) || (Sql.Equals(left, null) && Sql.NotEquals(right, null)) || (Sql.NotEquals(left, null) && Sql.Equals(right, null)) */
            var e = DbExpression.Or(left_not_equals_right, left_is_null_and_right_is_not_null_or_left_is_not_null_and_right_is_null);

            e.Accept(this);

            return exp;
        }

        public override DbExpression Visit(DbNotExpression exp)
        {
            this.SqlBuilder.Append("NOT ");
            this.SqlBuilder.Append("(");
            exp.Operand.Accept(this);
            this.SqlBuilder.Append(")");

            return exp;
        }

        public override DbExpression Visit(DbBitAndExpression exp)
        {
            this.SqlBuilder.Append("BITAND(");
            exp.Left.Accept(this);
            this.SqlBuilder.Append(",");
            exp.Left.Accept(this);
            this.SqlBuilder.Append(")");

            return exp;
        }
        public override DbExpression Visit(DbAndExpression exp)
        {
            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " AND ");

            return exp;
        }
        public override DbExpression Visit(DbBitOrExpression exp)
        {
            throw new NotSupportedException("'|' operator is not supported.");
        }
        public override DbExpression Visit(DbOrExpression exp)
        {
            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " OR ");

            return exp;
        }

        // +
        public override DbExpression Visit(DbAddExpression exp)
        {
            MethodInfo method = exp.Method;
            if (method != null)
            {
                Action<DbBinaryExpression, SqlGeneratorBase> handler;
                if (BinaryWithMethodHandlers.TryGetValue(method, out handler))
                {
                    handler(exp, this);
                    return exp;
                }
            }

            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " + ");

            return exp;
        }
        // -
        public override DbExpression Visit(DbSubtractExpression exp)
        {
            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " - ");

            return exp;
        }
        // *
        public override DbExpression Visit(DbMultiplyExpression exp)
        {
            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " * ");

            return exp;
        }
        // /
        public override DbExpression Visit(DbDivideExpression exp)
        {
            Stack<DbExpression> operands = GatherBinaryExpressionOperand(exp);
            this.ConcatOperands(operands, " / ");

            return exp;
        }
        // %
        public override DbExpression Visit(DbModuloExpression exp)
        {
            this.SqlBuilder.Append("MOD(");
            exp.Left.Accept(this);
            this.SqlBuilder.Append(",");
            exp.Right.Accept(this);
            this.SqlBuilder.Append(")");

            return exp;
        }
        public override DbExpression Visit(DbNegateExpression exp)
        {
            this.SqlBuilder.Append("(");

            this.SqlBuilder.Append("-");
            exp.Operand.Accept(this);

            this.SqlBuilder.Append(")");
            return exp;
        }
        // <
        public override DbExpression Visit(DbLessThanExpression exp)
        {
            exp.Left.Accept(this);
            this.SqlBuilder.Append(" < ");
            exp.Right.Accept(this);

            return exp;
        }
        // <=
        public override DbExpression Visit(DbLessThanOrEqualExpression exp)
        {
            exp.Left.Accept(this);
            this.SqlBuilder.Append(" <= ");
            exp.Right.Accept(this);

            return exp;
        }
        // >
        public override DbExpression Visit(DbGreaterThanExpression exp)
        {
            exp.Left.Accept(this);
            this.SqlBuilder.Append(" > ");
            exp.Right.Accept(this);

            return exp;
        }
        // >=
        public override DbExpression Visit(DbGreaterThanOrEqualExpression exp)
        {
            exp.Left.Accept(this);
            this.SqlBuilder.Append(" >= ");
            exp.Right.Accept(this);

            return exp;
        }


        public override DbExpression Visit(DbAggregateExpression exp)
        {
            Action<DbAggregateExpression, SqlGeneratorBase> aggregateHandler;
            if (!AggregateHandlers.TryGetValue(exp.Method.Name, out aggregateHandler))
            {
                throw UtilExceptions.NotSupportedMethod(exp.Method);
            }

            aggregateHandler(exp, this);
            return exp;
        }


        public override DbExpression Visit(DbTableExpression exp)
        {
            this.AppendTable(exp.Table);
            return exp;
        }
        public override DbExpression Visit(DbColumnAccessExpression exp)
        {
            this.QuoteName(exp.Table.Name);
            this.SqlBuilder.Append(".");
            this.QuoteName(exp.Column.Name);

            return exp;
        }
        public override DbExpression Visit(DbFromTableExpression exp)
        {
            this.AppendTableSegment(exp.Table);
            this.VisitDbJoinTableExpressions(exp.JoinTables);

            return exp;
        }
        public override DbExpression Visit(DbJoinTableExpression exp)
        {
            DbJoinTableExpression joinTablePart = exp;
            string joinString = null;

            if (joinTablePart.JoinType == DbJoinType.InnerJoin)
            {
                joinString = " INNER JOIN ";
            }
            else if (joinTablePart.JoinType == DbJoinType.LeftJoin)
            {
                joinString = " LEFT JOIN ";
            }
            else if (joinTablePart.JoinType == DbJoinType.RightJoin)
            {
                joinString = " RIGHT JOIN ";
            }
            else if (joinTablePart.JoinType == DbJoinType.FullJoin)
            {
                joinString = " FULL JOIN ";
            }
            else
                throw new NotSupportedException("JoinType: " + joinTablePart.JoinType);

            this.SqlBuilder.Append(joinString);
            this.AppendTableSegment(joinTablePart.Table);
            this.SqlBuilder.Append(" ON ");
            JoinConditionExpressionTransformer.Transform(joinTablePart.Condition).Accept(this);
            this.VisitDbJoinTableExpressions(joinTablePart.JoinTables);

            return exp;
        }


        public override DbExpression Visit(DbSubQueryExpression exp)
        {
            this.SqlBuilder.Append("(");
            exp.SqlQuery.Accept(this);
            this.SqlBuilder.Append(")");

            return exp;
        }
        public override DbExpression Visit(DbSqlQueryExpression exp)
        {
            if (exp.TakeCount != null)
            {
                DbSqlQueryExpression newSqlQuery = CloneWithoutLimitInfo(exp, "TTAKE");

                if (exp.SkipCount == null)
                    AppendLimitCondition(newSqlQuery, exp.TakeCount.Value);
                else
                {
                    AppendLimitCondition(newSqlQuery, exp.TakeCount.Value + exp.SkipCount.Value);
                    newSqlQuery.SkipCount = exp.SkipCount.Value;
                }

                newSqlQuery.IsDistinct = exp.IsDistinct;
                newSqlQuery.Accept(this);
                return exp;
            }
            else if (exp.SkipCount != null)
            {
                DbSqlQueryExpression subSqlQuery = CloneWithoutLimitInfo(exp, "TSKIP");

                string row_numberName = GenRowNumberName(subSqlQuery.ColumnSegments);
                DbColumnSegment row_numberSeg = new DbColumnSegment(OracleSemantics.DbMemberExpression_ROWNUM, row_numberName);
                subSqlQuery.ColumnSegments.Add(row_numberSeg);

                DbTable table = new DbTable("T");
                DbSqlQueryExpression newSqlQuery = WrapSqlQuery(subSqlQuery, table, exp.ColumnSegments);

                DbColumnAccessExpression columnAccessExp = new DbColumnAccessExpression(table, DbColumn.MakeColumn(row_numberSeg.Body, row_numberName));
                newSqlQuery.Condition = DbExpression.GreaterThan(columnAccessExp, DbExpression.Constant(exp.SkipCount.Value));

                newSqlQuery.IsDistinct = exp.IsDistinct;
                newSqlQuery.Accept(this);
                return exp;
            }

            this.BuildGeneralSql(exp);
            return exp;
        }
        public override DbExpression Visit(DbInsertExpression exp)
        {
            this.SqlBuilder.Append("INSERT INTO ");
            this.AppendTable(exp.Table);
            this.SqlBuilder.Append("(");

            bool first = true;
            foreach (var item in exp.InsertColumns)
            {
                if (first)
                    first = false;
                else
                {
                    this.SqlBuilder.Append(",");
                }

                this.QuoteName(item.Key.Name);
            }

            this.SqlBuilder.Append(")");

            this.SqlBuilder.Append(" VALUES(");
            first = true;
            foreach (var item in exp.InsertColumns)
            {
                if (first)
                    first = false;
                else
                {
                    this.SqlBuilder.Append(",");
                }

                DbExpression valExp = item.Value.StripInvalidConvert();
                AmendDbInfo(item.Key, valExp);
                DbValueExpressionTransformer.Transform(valExp).Accept(this);
            }

            this.SqlBuilder.Append(")");

            if (exp.Returns.Count > 0)
            {
                this.SqlBuilder.Append(" RETURNING ");

                string outputParamNames = "";
                for (int i = 0; i < exp.Returns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.SqlBuilder.Append(",");
                        outputParamNames = outputParamNames + ",";
                    }

                    DbColumn outputColumn = exp.Returns[i];
                    string paramName = Utils.GenOutputColumnParameterName(outputColumn.Name);
                    DbParam outputParam = new DbParam() { Name = paramName, DbType = outputColumn.DbType, Precision = outputColumn.Precision, Scale = outputColumn.Scale, Size = outputColumn.Size, Value = DBNull.Value, Direction = ParamDirection.Output };
                    outputParam.Type = outputColumn.Type;

                    this.QuoteName(outputColumn.Name);
                    outputParamNames = outputParamNames + paramName;

                    this._parameters.Add(outputParam);
                }

                this.SqlBuilder.Append(" INTO ", outputParamNames);
            }

            return exp;
        }
        public override DbExpression Visit(DbUpdateExpression exp)
        {
            this.SqlBuilder.Append("UPDATE ");
            this.AppendTable(exp.Table);
            this.SqlBuilder.Append(" SET ");

            bool first = true;
            foreach (var item in exp.UpdateColumns)
            {
                if (first)
                    first = false;
                else
                    this.SqlBuilder.Append(",");

                this.QuoteName(item.Key.Name);
                this.SqlBuilder.Append("=");

                DbExpression valExp = item.Value.StripInvalidConvert();
                AmendDbInfo(item.Key, valExp);
                DbValueExpressionTransformer.Transform(valExp).Accept(this);
            }

            this.BuildWhereState(exp.Condition);

            return exp;
        }
        public override DbExpression Visit(DbDeleteExpression exp)
        {
            this.SqlBuilder.Append("DELETE FROM ");
            this.AppendTable(exp.Table);
            this.BuildWhereState(exp.Condition);

            return exp;
        }

        public override DbExpression Visit(DbExistsExpression exp)
        {
            this.SqlBuilder.Append("Exists ");

            DbSqlQueryExpression rawSqlQuery = exp.SqlQuery;
            DbSqlQueryExpression sqlQuery = new DbSqlQueryExpression()
            {
                TakeCount = rawSqlQuery.TakeCount,
                SkipCount = rawSqlQuery.SkipCount,
                Table = rawSqlQuery.Table,
                Condition = rawSqlQuery.Condition,
                HavingCondition = rawSqlQuery.HavingCondition,
            };

            sqlQuery.GroupSegments.AddRange(rawSqlQuery.GroupSegments);

            DbColumnSegment columnSegment = new DbColumnSegment(DbExpression.Parameter("1"), "C");
            sqlQuery.ColumnSegments.Add(columnSegment);

            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);
            return subQuery.Accept(this);
        }

        public override DbExpression Visit(DbCoalesceExpression exp)
        {
            this.SqlBuilder.Append("NVL(");
            EnsureDbExpressionReturnCSharpBoolean(exp.CheckExpression).Accept(this);
            this.SqlBuilder.Append(",");
            EnsureDbExpressionReturnCSharpBoolean(exp.ReplacementValue).Accept(this);
            this.SqlBuilder.Append(")");

            return exp;
        }
        // then 部分必须返回 C# type，所以得判断是否是诸如 a>1,a=b,in,like 等等的情况，如果是则将其构建成一个 case when 
        public override DbExpression Visit(DbCaseWhenExpression exp)
        {
            this.LeftBracket();

            this.SqlBuilder.Append("CASE");
            foreach (var whenThen in exp.WhenThenPairs)
            {
                // then 部分得判断是否是诸如 a>1,a=b,in,like 等等的情况，如果是则将其构建成一个 case when 
                this.SqlBuilder.Append(" WHEN ");
                whenThen.When.Accept(this);
                this.SqlBuilder.Append(" THEN ");
                EnsureDbExpressionReturnCSharpBoolean(whenThen.Then).Accept(this);
            }

            this.SqlBuilder.Append(" ELSE ");
            EnsureDbExpressionReturnCSharpBoolean(exp.Else).Accept(this);
            this.SqlBuilder.Append(" END");

            this.RightBracket();

            return exp;
        }
        public override DbExpression Visit(DbConvertExpression exp)
        {
            DbExpression stripedExp = DbExpressionExtension.StripInvalidConvert(exp);

            if (stripedExp.NodeType != DbExpressionType.Convert)
            {
                EnsureDbExpressionReturnCSharpBoolean(stripedExp).Accept(this);
                return exp;
            }

            exp = (DbConvertExpression)stripedExp;

            if (exp.Type == PublicConstants.TypeOfString)
            {
                this.SqlBuilder.Append("TO_CHAR(");
                exp.Operand.Accept(this);
                this.SqlBuilder.Append(")");
                return exp;
            }

            if (exp.Type == PublicConstants.TypeOfDateTime)
            {
                this.SqlBuilder.Append("TO_TIMESTAMP(");
                exp.Operand.Accept(this);
                this.SqlBuilder.Append(",'yyyy-mm-dd hh24:mi:ssxff')");
                return exp;
            }

            string dbTypeString;
            if (TryGetCastTargetDbTypeString(exp.Operand.Type, exp.Type, out dbTypeString, false))
            {
                this.BuildCastState(EnsureDbExpressionReturnCSharpBoolean(exp.Operand), dbTypeString);
            }
            else
                EnsureDbExpressionReturnCSharpBoolean(exp.Operand).Accept(this);

            return exp;
        }


        public override DbExpression Visit(DbMethodCallExpression exp)
        {
            IMethodHandler methodHandler;
            if (MethodHandlers.TryGetValue(exp.Method.Name, out methodHandler))
            {
                if (methodHandler.CanProcess(exp))
                {
                    methodHandler.Process(exp, this);
                    return exp;
                }
            }

            DbFunctionAttribute dbFunction = exp.Method.GetCustomAttribute<DbFunctionAttribute>();
            if (dbFunction != null)
            {
                string schema = dbFunction.Schema;
                string functionName = string.IsNullOrEmpty(dbFunction.Name) ? exp.Method.Name : dbFunction.Name;

                if (!string.IsNullOrEmpty(schema))
                {
                    this.QuoteName(schema);
                    this.SqlBuilder.Append(".");
                }

                this.QuoteName(functionName);
                this.SqlBuilder.Append("(");

                string c = "";
                foreach (DbExpression argument in exp.Arguments)
                {
                    this.SqlBuilder.Append(c);
                    argument.Accept(this);
                    c = ",";
                }

                this.SqlBuilder.Append(")");

                return exp;
            }

            if (exp.IsEvaluable())
            {
                DbParameterExpression dbParameter = new DbParameterExpression(exp.Evaluate(), exp.Type);
                return dbParameter.Accept(this);
            }

            throw UtilExceptions.NotSupportedMethod(exp.Method);
        }
        public override DbExpression Visit(DbMemberExpression exp)
        {
            MemberInfo member = exp.Member;

            if (member == OracleSemantics.PropertyInfo_ROWNUM)
            {
                this.SqlBuilder.Append("ROWNUM");
                return exp;
            }

            if (member.DeclaringType == PublicConstants.TypeOfDateTime)
            {
                if (member == PublicConstants.PropertyInfo_DateTime_Now)
                {
                    this.SqlBuilder.Append("SYSTIMESTAMP");
                    return exp;
                }

                if (member == PublicConstants.PropertyInfo_DateTime_UtcNow)
                {
                    this.SqlBuilder.Append("SYS_EXTRACT_UTC(SYSTIMESTAMP)");
                    return exp;
                }

                if (member == PublicConstants.PropertyInfo_DateTime_Today)
                {
                    //other way: this.SqlBuilder.Append("TO_DATE(TO_CHAR(SYSDATE,'yyyy-mm-dd'),'yyyy-mm-dd')");
                    this.SqlBuilder.Append("TRUNC(SYSDATE,'DD')");
                    return exp;
                }

                if (member == PublicConstants.PropertyInfo_DateTime_Date)
                {
                    this.SqlBuilder.Append("TRUNC(");
                    exp.Expression.Accept(this);
                    this.SqlBuilder.Append(",'DD')");
                    return exp;
                }

                if (this.IsDatePart(exp))
                {
                    return exp;
                }
            }

            if (this.IsDateSubtract(exp))
            {
                return exp;
            }

            if (member.Name == "Length" && member.DeclaringType == PublicConstants.TypeOfString)
            {
                this.SqlBuilder.Append("LENGTH(");
                exp.Expression.Accept(this);
                this.SqlBuilder.Append(")");

                return exp;
            }

            if (member.Name == "Value" && ReflectionExtension.IsNullable(exp.Expression.Type))
            {
                exp.Expression.Accept(this);
                return exp;
            }

            DbParameterExpression newExp;
            if (DbExpressionExtension.TryConvertToParameterExpression(exp, out newExp))
            {
                return newExp.Accept(this);
            }

            throw new NotSupportedException(string.Format("'{0}.{1}' is not supported.", member.DeclaringType.FullName, member.Name));
        }
        public override DbExpression Visit(DbConstantExpression exp)
        {
            if (exp.Value == null || exp.Value == DBNull.Value)
            {
                this.SqlBuilder.Append("NULL");
                return exp;
            }

            var objType = exp.Value.GetType();
            if (objType == PublicConstants.TypeOfBoolean)
            {
                this.SqlBuilder.Append(((bool)exp.Value) ? "1" : "0");
                return exp;
            }
            else if (objType == PublicConstants.TypeOfString)
            {
                if (string.Empty.Equals(exp.Value))
                    this.SqlBuilder.Append("'", exp.Value, "'");
                else
                    this.SqlBuilder.Append("N'", exp.Value, "'");
                return exp;
            }
            else if (objType.IsEnum)
            {
                this.SqlBuilder.Append(Convert.ChangeType(exp.Value, Enum.GetUnderlyingType(objType)).ToString());
                return exp;
            }
            else if (NumericTypes.ContainsKey(exp.Value.GetType()))
            {
                this.SqlBuilder.Append(exp.Value);
                return exp;
            }

            DbParameterExpression p = new DbParameterExpression(exp.Value);
            p.Accept(this);

            return exp;
        }
        public override DbExpression Visit(DbParameterExpression exp)
        {
            object paramValue = exp.Value;
            Type paramType = exp.Type.GetUnderlyingType();

            if (paramType.IsEnum)
            {
                paramType = Enum.GetUnderlyingType(paramType);
                if (paramValue != null)
                    paramValue = Convert.ChangeType(paramValue, paramType);
            }
            else if (paramType == PublicConstants.TypeOfBoolean)
            {
                paramType = PublicConstants.TypeOfInt32;
                if (paramValue != null)
                {
                    paramValue = (bool)paramValue ? Boxed_1 : Boxed_0;
                }

                if (exp.DbType == null || exp.DbType == DbType.Boolean)
                {
                    exp.DbType = DbType.Int32;
                }
            }

            if (paramValue == null)
                paramValue = DBNull.Value;

            DbParam p = this._parameters.Find(paramValue, paramType, exp.DbType);

            if (p != null)
            {
                this.SqlBuilder.Append(p.Name);
                return exp;
            }

            string paramName = GenParameterName(this._parameters.Count);
            p = DbParam.Create(paramName, paramValue, paramType);

            if (paramValue.GetType() == PublicConstants.TypeOfString)
            {
                if (exp.DbType == DbType.AnsiStringFixedLength || exp.DbType == DbType.StringFixedLength)
                    p.Size = ((string)paramValue).Length;
                else if (((string)paramValue).Length <= 4000)
                    p.Size = 4000;
            }

            if (exp.DbType != null)
                p.DbType = exp.DbType;

            this._parameters.Add(p);
            this.SqlBuilder.Append(paramName);
            return exp;
        }


        void AppendTableSegment(DbTableSegment seg)
        {
            seg.Body.Accept(this);
            this.SqlBuilder.Append(" ");
            this.QuoteName(seg.Alias);
        }
        void AppendColumnSegment(DbColumnSegment seg)
        {
            DbValueExpressionTransformer.Transform(seg.Body).Accept(this);
            this.SqlBuilder.Append(" AS ");
            this.QuoteName(seg.Alias);
        }
        void AppendOrdering(DbOrdering ordering)
        {
            if (ordering.OrderType == DbOrderType.Asc)
            {
                ordering.Expression.Accept(this);
                this.SqlBuilder.Append(" ASC");
                return;
            }
            else if (ordering.OrderType == DbOrderType.Desc)
            {
                ordering.Expression.Accept(this);
                this.SqlBuilder.Append(" DESC");
                return;
            }

            throw new NotSupportedException("OrderType: " + ordering.OrderType);
        }

        void VisitDbJoinTableExpressions(List<DbJoinTableExpression> tables)
        {
            foreach (var table in tables)
            {
                table.Accept(this);
            }
        }
        void BuildGeneralSql(DbSqlQueryExpression exp)
        {
            if (exp.TakeCount != null || exp.SkipCount != null)
                throw new ArgumentException();

            this.SqlBuilder.Append("SELECT ");

            if (exp.IsDistinct)
                this.SqlBuilder.Append("DISTINCT ");

            List<DbColumnSegment> columns = exp.ColumnSegments;
            for (int i = 0; i < columns.Count; i++)
            {
                DbColumnSegment column = columns[i];
                if (i > 0)
                    this.SqlBuilder.Append(",");

                this.AppendColumnSegment(column);
            }

            this.SqlBuilder.Append(" FROM ");
            exp.Table.Accept(this);
            this.BuildWhereState(exp.Condition);
            this.BuildGroupState(exp);
            this.BuildOrderState(exp.Orderings);

            DbTableSegment seg = exp.Table.Table;
            if (seg.Lock == LockType.UpdLock)
            {
                this.SqlBuilder.Append(" FOR UPDATE");
            }
            else if (seg.Lock == LockType.Unspecified || seg.Lock == LockType.NoLock)
            {
                //Do nothing.
            }
            else
                throw new NotSupportedException($"lock type: {seg.Lock.ToString()}");
        }


        void BuildWhereState(DbExpression whereExpression)
        {
            if (whereExpression != null)
            {
                this.SqlBuilder.Append(" WHERE ");
                whereExpression.Accept(this);
            }
        }
        void BuildOrderState(List<DbOrdering> orderings)
        {
            if (orderings.Count > 0)
            {
                this.SqlBuilder.Append(" ORDER BY ");
                this.ConcatOrderings(orderings);
            }
        }
        void ConcatOrderings(List<DbOrdering> orderings)
        {
            for (int i = 0; i < orderings.Count; i++)
            {
                if (i > 0)
                {
                    this.SqlBuilder.Append(",");
                }

                this.AppendOrdering(orderings[i]);
            }
        }
        void BuildGroupState(DbSqlQueryExpression exp)
        {
            var groupSegments = exp.GroupSegments;
            if (groupSegments.Count == 0)
                return;

            this.SqlBuilder.Append(" GROUP BY ");
            for (int i = 0; i < groupSegments.Count; i++)
            {
                if (i > 0)
                    this.SqlBuilder.Append(",");

                groupSegments[i].Accept(this);
            }

            if (exp.HavingCondition != null)
            {
                this.SqlBuilder.Append(" HAVING ");
                exp.HavingCondition.Accept(this);
            }
        }

        public virtual string SqlName(string name)
        {
            return name;
        }
        public virtual void QuoteName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            this.SqlBuilder.Append("\"", this.SqlName(name), "\"");
        }
        void AppendTable(DbTable table)
        {
            if (!string.IsNullOrEmpty(table.Schema))
            {
                this.QuoteName(table.Schema);
                this.SqlBuilder.Append(".");
            }

            this.QuoteName(table.Name);
        }

        void ConcatOperands(IEnumerable<DbExpression> operands, string connector)
        {
            this.SqlBuilder.Append("(");

            bool first = true;
            foreach (DbExpression operand in operands)
            {
                if (first)
                    first = false;
                else
                    this.SqlBuilder.Append(connector);

                operand.Accept(this);
            }

            this.SqlBuilder.Append(")");
            return;
        }
        void BuildCastState(DbExpression castExp, string targetDbTypeString)
        {
            this.SqlBuilder.Append("CAST(");
            castExp.Accept(this);
            this.SqlBuilder.Append(" AS ", targetDbTypeString, ")");
        }


        bool IsDatePart(DbMemberExpression exp)
        {
            MemberInfo member = exp.Member;

            if (member == PublicConstants.PropertyInfo_DateTime_Year)
            {
                DbFunction_DATEPART(this, "yyyy", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Month)
            {
                DbFunction_DATEPART(this, "mm", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Day)
            {
                DbFunction_DATEPART(this, "dd", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Hour)
            {
                DbFunction_DATEPART(this, "hh24", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Minute)
            {
                DbFunction_DATEPART(this, "mi", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Second)
            {
                DbFunction_DATEPART(this, "ss", exp.Expression);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_Millisecond)
            {
                /* exp.Expression must be TIMESTAMP,otherwise there will be an error occurred. */
                DbFunction_DATEPART(this, "ff3", exp.Expression, true);
                return true;
            }

            if (member == PublicConstants.PropertyInfo_DateTime_DayOfWeek)
            {
                // CAST(TO_CHAR(SYSDATE,'D') AS NUMBER) - 1
                this.SqlBuilder.Append("(");
                DbFunction_DATEPART(this, "D", exp.Expression);
                this.SqlBuilder.Append(" - 1");
                this.SqlBuilder.Append(")");

                return true;
            }

            return false;
        }
        bool IsDateSubtract(DbMemberExpression exp)
        {
            MemberInfo member = exp.Member;

            if (member.DeclaringType == PublicConstants.TypeOfTimeSpan)
            {
                if (exp.Expression.NodeType == DbExpressionType.Call)
                {
                    DbMethodCallExpression dbMethodExp = (DbMethodCallExpression)exp.Expression;
                    if (dbMethodExp.Method == PublicConstants.MethodInfo_DateTime_Subtract_DateTime)
                    {
                        int? intervalDivisor = null;

                        if (member == UtilConstants.PropertyInfo_TimeSpan_TotalDays)
                        {
                            intervalDivisor = 24 * 60 * 60 * 1000;
                            goto appendIntervalTime;
                        }
                        if (member == UtilConstants.PropertyInfo_TimeSpan_TotalHours)
                        {
                            intervalDivisor = 60 * 60 * 1000;
                            goto appendIntervalTime;
                        }
                        if (member == UtilConstants.PropertyInfo_TimeSpan_TotalMinutes)
                        {
                            intervalDivisor = 60 * 1000;
                            goto appendIntervalTime;
                        }
                        if (member == UtilConstants.PropertyInfo_TimeSpan_TotalSeconds)
                        {
                            intervalDivisor = 1000;
                            goto appendIntervalTime;
                        }
                        if (member == UtilConstants.PropertyInfo_TimeSpan_TotalMilliseconds)
                        {
                            intervalDivisor = 1;
                            goto appendIntervalTime;
                        }

                        return false;

appendIntervalTime:
                        this.CalcDateDiffPrecise(dbMethodExp.Object, dbMethodExp.Arguments[0], intervalDivisor.Value);
                        return true;
                    }
                }
                else
                {
                    DbSubtractExpression dbSubtractExp = exp.Expression as DbSubtractExpression;
                    if (dbSubtractExp != null && dbSubtractExp.Left.Type == PublicConstants.TypeOfDateTime && dbSubtractExp.Right.Type == PublicConstants.TypeOfDateTime)
                    {
                        DbMethodCallExpression dbMethodExp = new DbMethodCallExpression(dbSubtractExp.Left, PublicConstants.MethodInfo_DateTime_Subtract_DateTime, new List<DbExpression>(1) { dbSubtractExp.Right });
                        DbMemberExpression dbMemberExp = DbExpression.MemberAccess(member, dbMethodExp);
                        dbMemberExp.Accept(this);

                        return true;
                    }
                }
            }

            return false;
        }

        void CalcDateDiffPrecise(DbExpression dateTime1, DbExpression dateTime2, int divisor)
        {
            if (divisor == 1)
            {
                this.CalcDateDiffMillisecond(dateTime1, dateTime2);
                return;
            }

            this.LeftBracket();
            this.CalcDateDiffMillisecond(dateTime1, dateTime2);
            this.SqlBuilder.Append(" / ");
            this.SqlBuilder.Append(divisor.ToString());
            this.RightBracket();
        }
        void CalcDateDiffMillisecond(DbExpression dateTime1, DbExpression dateTime2)
        {
            /*
             * 计算两个日期相差的毫秒数：
             * (cast(dateTime1 as date)-cast(dateTime2 as date)) * 24 * 60 * 60 * 1000 
             * +
             * cast(to_char(cast(dateTime1 as timestamp),'ff3') as number)
             * -
             * cast(to_char(cast(dateTime2 as timestamp),'ff3') as number) 
             */

            this.LeftBracket();
            this.CalcDateDiffMillisecondSketchy(dateTime1, dateTime2);
            this.SqlBuilder.Append(" + ");
            this.ExtractMillisecondPart(dateTime1);
            this.SqlBuilder.Append(" - ");
            this.ExtractMillisecondPart(dateTime2);
            this.RightBracket();
        }
        void CalcDateDiffMillisecondSketchy(DbExpression dateTime1, DbExpression dateTime2)
        {
            /*
             * 计算去掉毫秒部分后两个日期相差的毫秒数：
             * (cast(dateTime1 as date)-cast(dateTime2 as date)) * 24 * 60 * 60 * 1000 
             */
            this.LeftBracket();
            this.BuildCastState(dateTime1, "DATE");
            this.SqlBuilder.Append("-");
            this.BuildCastState(dateTime2, "DATE");
            this.RightBracket();

            this.SqlBuilder.Append(" * ");
            this.SqlBuilder.Append((24 * 60 * 60 * 1000).ToString());
        }
        void ExtractMillisecondPart(DbExpression dateTime)
        {
            /* 提取一个日期的毫秒部分：
             * cast(to_char(cast(dateTime as timestamp),'ff3') as number) 
             */
            this.SqlBuilder.Append("CAST(");

            this.SqlBuilder.Append("TO_CHAR(");
            this.BuildCastState(dateTime, "TIMESTAMP");
            this.SqlBuilder.Append(",'ff3')");

            this.SqlBuilder.Append(" AS NUMBER)");
        }
    }
}
