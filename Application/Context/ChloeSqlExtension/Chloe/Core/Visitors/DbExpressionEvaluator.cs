using Chloe.DbExpressions;
using Chloe.Reflection;

namespace Chloe.Core.Visitors
{
    public class DbExpressionEvaluator : DbExpressionVisitor<object>
    {
        static DbExpressionEvaluator _evaluator = new DbExpressionEvaluator();
        public static object Evaluate(DbExpression exp)
        {
            return _evaluator.Visit(exp);
        }

        object Visit(DbExpression exp)
        {
            if (exp == null)
                throw new ArgumentNullException();

            return exp.Accept(_evaluator);
        }

        public override object Visit(DbConstantExpression exp)
        {
            return exp.Value;
        }
        public override object Visit(DbMemberExpression exp)
        {
            object instance = null;
            if (exp.Expression != null)
            {
                instance = exp.Expression.Accept(this);

                if (instance == null)
                {
                    if (exp.Member.Name == "HasValue" && exp.Member.DeclaringType.IsNullable())
                    {
                        return false;
                    }

                    throw new NullReferenceException(string.Format("There is an object reference not set to an instance in expression tree. The type of null object is '{0}'.", exp.Expression.Type.FullName));
                }
            }

            return exp.Member.FastGetMemberValue(instance);
        }
        public override object Visit(DbMethodCallExpression exp)
        {
            object instance = null;
            if (exp.Object != null)
            {
                instance = exp.Object.Accept(this);

                if (instance == null)
                {
                    throw new NullReferenceException(string.Format("There is an object reference not set to an instance in expression tree. The type of null object is '{0}'.", exp.Object.Type.FullName));
                }
            }

            object[] arguments = exp.Arguments.Select(a => a.Accept(this)).ToArray();

            return exp.Method.FastInvoke(instance, arguments);
        }
        public override object Visit(DbNotExpression exp)
        {
            var operandValue = exp.Operand.Accept(this);

            if ((bool)operandValue == true)
                return false;

            return true;
        }
        public override object Visit(DbConvertExpression exp)
        {
            object operandValue = exp.Operand.Accept(this);

            //(int)null
            if (operandValue == null)
            {
                //(int)null
                if (exp.Type.IsValueType && !exp.Type.IsNullable())
                    throw new NullReferenceException();

                return null;
            }

            Type operandValueType = operandValue.GetType();

            if (exp.Type == operandValueType || exp.Type.IsAssignableFrom(operandValueType))
            {
                return operandValue;
            }

            Type underlyingType;

            if (exp.Type.IsNullable(out underlyingType))
            {
                //(int?)int
                if (underlyingType == operandValueType)
                {
                    var constructor = exp.Type.GetConstructor(new Type[] { operandValueType });
                    var val = constructor.Invoke(new object[] { operandValue });
                    return val;
                }
                else
                {
                    //如果不等，则诸如：(long?)int / (long?)int?  -->  (long?)((long)int) / (long?)((long)int?)
                    var c = DbExpression.Convert(DbExpression.Constant(operandValue), underlyingType);
                    var cc = DbExpression.Convert(c, exp.Type);
                    return this.Visit(cc);
                }
            }

            //(int)int?
            if (operandValueType.IsNullable(out underlyingType))
            {
                if (underlyingType == exp.Type)
                {
                    var pro = operandValueType.GetProperty("Value");
                    var val = pro.GetValue(operandValue, null);
                    return val;
                }
                else
                {
                    //如果不等，则诸如：(long)int?  -->  (long)((long)int)
                    var c = DbExpression.Convert(DbExpression.Constant(operandValue), underlyingType);
                    var cc = DbExpression.Convert(c, exp.Type);
                    return this.Visit(cc);
                }
            }

            if (exp.Type.IsEnum)
            {
                return Enum.ToObject(exp.Type, operandValue);
            }

            //(long)int
            if (operandValue is IConvertible)
            {
                return Convert.ChangeType(operandValue, exp.Type);
            }

            throw new NotSupportedException(string.Format("Does not support the type '{0}' converted to type '{1}'.", operandValueType.FullName, exp.Type.FullName));
        }
        public override object Visit(DbParameterExpression exp)
        {
            return exp.Value;
        }
        public override object Visit(DbCoalesceExpression exp)
        {
            object left = exp.CheckExpression.Accept(this);
            if (left == null)
                return exp.ReplacementValue.Accept(this);

            return left;
        }
    }
}
