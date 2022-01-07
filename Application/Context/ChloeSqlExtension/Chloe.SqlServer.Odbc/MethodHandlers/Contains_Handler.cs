using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using Chloe.RDBMS;
using System.Collections;
using System.Reflection;

namespace Chloe.SqlServer.Odbc.MethodHandlers
{
    class Contains_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            MethodInfo method = exp.Method;

            if (exp.Method == PublicConstants.MethodInfo_String_Contains)
            {
                return true;
            }

            Type declaringType = method.DeclaringType;
            if (typeof(IList).IsAssignableFrom(declaringType) || (declaringType.IsGenericType && typeof(ICollection<>).MakeGenericType(declaringType.GetGenericArguments()).IsAssignableFrom(declaringType)))
            {
                return true;
            }
            if (method.IsStatic && declaringType == typeof(Enumerable) && exp.Arguments.Count == 2)
            {
                return true;
            }

            return false;
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            MethodInfo method = exp.Method;

            if (exp.Method == PublicConstants.MethodInfo_String_Contains)
            {
                Method_String_Contains(exp, generator);
                return;
            }

            List<DbExpression> exps = new List<DbExpression>();
            IEnumerable values = null;
            DbExpression operand = null;

            Type declaringType = method.DeclaringType;

            if (typeof(IList).IsAssignableFrom(declaringType) || (declaringType.IsGenericType && typeof(ICollection<>).MakeGenericType(declaringType.GetGenericArguments()).IsAssignableFrom(declaringType)))
            {
                if (exp.Object.NodeType == DbExpressionType.SqlQuery)
                {
                    /* where Id in(select id from T) */

                    operand = exp.Arguments[0];
                    In(generator, (DbSqlQueryExpression)exp.Object, operand);
                    return;
                }

                if (!exp.Object.IsEvaluable())
                    throw new NotSupportedException(exp.ToString());

                values = DbExpressionExtension.Evaluate(exp.Object) as IEnumerable; //Enumerable
                operand = exp.Arguments[0];
                goto constructInState;
            }
            if (method.IsStatic && declaringType == typeof(Enumerable) && exp.Arguments.Count == 2)
            {
                DbExpression arg0 = exp.Arguments[0];
                if (arg0.NodeType == DbExpressionType.SqlQuery)
                {
                    /* where Id in(select id from T) */

                    operand = exp.Arguments[1];
                    In(generator, (DbSqlQueryExpression)arg0, operand);
                    return;
                }

                if (!arg0.IsEvaluable())
                    throw UtilExceptions.NotSupportedMethod(exp.Method);

                values = DbExpressionExtension.Evaluate(arg0) as IEnumerable;
                operand = exp.Arguments[1];
                goto constructInState;
            }

            throw UtilExceptions.NotSupportedMethod(exp.Method);

        constructInState:
            foreach (object value in values)
            {
                if (value == null)
                    exps.Add(DbExpression.Constant(null, operand.Type));
                else
                {
                    Type valueType = value.GetType();
                    if (valueType.IsEnum)
                        valueType = Enum.GetUnderlyingType(valueType);

                    if (Utils.IsToStringableNumericType(valueType))
                        exps.Add(DbExpression.Constant(value));
                    else
                        exps.Add(DbExpression.Parameter(value));
                }
            }

            In(generator, exps, operand);
        }

        static void Method_String_Contains(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            exp.Object.Accept(generator);
            generator.SqlBuilder.Append(" LIKE '%' + ");
            exp.Arguments.First().Accept(generator);
            generator.SqlBuilder.Append(" + '%'");
        }

        static void In(SqlGeneratorBase generator, List<DbExpression> elementExps, DbExpression operand)
        {
            if (elementExps.Count == 0)
            {
                generator.SqlBuilder.Append("1=0");
                return;
            }

            operand.Accept(generator);
            generator.SqlBuilder.Append(" IN (");

            for (int i = 0; i < elementExps.Count; i++)
            {
                if (i > 0)
                    generator.SqlBuilder.Append(",");

                elementExps[i].Accept(generator);
            }

            generator.SqlBuilder.Append(")");
        }
        static void In(SqlGeneratorBase generator, DbSqlQueryExpression sqlQuery, DbExpression operand)
        {
            operand.Accept(generator);
            generator.SqlBuilder.Append(" IN (");
            sqlQuery.Accept(generator);
            generator.SqlBuilder.Append(")");
        }
    }
}
