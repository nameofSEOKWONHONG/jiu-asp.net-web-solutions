using Chloe.Extensions;
using Chloe.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Core.Visitors
{
    public class ExpressionEvaluator : ExpressionVisitor<object>
    {
        static ExpressionEvaluator _evaluator = new ExpressionEvaluator();
        public static object Evaluate(Expression exp)
        {
            return _evaluator.Visit(exp);
        }

        protected override object VisitMemberAccess(MemberExpression exp)
        {
            object instance = null;
            if (exp.Expression != null)
            {
                instance = this.Visit(exp.Expression);

                if (instance == null)
                {
                    if (exp.Member.Name == "HasValue" && exp.Member.DeclaringType.IsNullable())
                    {
                        return false;
                    }

                    throw new NullReferenceException(string.Format("There is an object reference not set to an instance in expression tree. Associated expression: '{0}'.", exp.Expression.ToString()));
                }
            }

            return exp.Member.FastGetMemberValue(instance);
        }
        protected override object VisitUnary_Not(UnaryExpression exp)
        {
            var operandValue = this.Visit(exp.Operand);

            if ((bool)operandValue == true)
                return false;

            return true;
        }
        protected override object VisitUnary_Convert(UnaryExpression exp)
        {
            object operandValue = this.Visit(exp.Operand);

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
                    var c = Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(operandValue), underlyingType);
                    var cc = Expression.MakeUnary(ExpressionType.Convert, c, exp.Type);
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
                    var c = Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(operandValue), underlyingType);
                    var cc = Expression.MakeUnary(ExpressionType.Convert, c, exp.Type);
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
        protected override object VisitUnary_Quote(UnaryExpression exp)
        {
            var e = ExpressionExtension.StripQuotes(exp);
            return e;
        }
        protected override object VisitConstant(ConstantExpression exp)
        {
            return exp.Value;
        }
        protected override object VisitMethodCall(MethodCallExpression exp)
        {
            object instance = null;
            if (exp.Object != null)
            {
                instance = this.Visit(exp.Object);

                if (instance == null)
                {
                    throw new NullReferenceException(string.Format("There is an object reference not set to an instance in expression tree. Associated expression: '{0}'.", exp.Object.ToString()));
                }
            }

            object[] arguments = exp.Arguments.Select(a => this.Visit(a)).ToArray();

            return exp.Method.FastInvoke(instance, arguments);
        }
        protected override object VisitNew(NewExpression exp)
        {
            object[] arguments = exp.Arguments.Select(a => this.Visit(a)).ToArray();

            return exp.Constructor.Invoke(arguments);
        }
        protected override object VisitNewArray(NewArrayExpression exp)
        {
            Array arr = Array.CreateInstance(exp.Type.GetElementType(), exp.Expressions.Count);
            for (int i = 0; i < exp.Expressions.Count; i++)
            {
                var e = exp.Expressions[i];
                arr.SetValue(this.Visit(e), i);
            }

            return arr;
        }
        protected override object VisitMemberInit(MemberInitExpression exp)
        {
            object instance = this.Visit(exp.NewExpression);

            for (int i = 0; i < exp.Bindings.Count; i++)
            {
                MemberBinding binding = exp.Bindings[i];

                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }

                MemberAssignment memberAssignment = (MemberAssignment)binding;
                MemberInfo member = memberAssignment.Member;

                member.SetMemberValue(instance, this.Visit(memberAssignment.Expression));
            }

            return instance;
        }
        protected override object VisitListInit(ListInitExpression exp)
        {
            object instance = this.Visit(exp.NewExpression);

            for (int i = 0; i < exp.Initializers.Count; i++)
            {
                ElementInit initializer = exp.Initializers[i];

                foreach (Expression argument in initializer.Arguments)
                {
                    initializer.AddMethod.FastInvoke(instance, this.Visit(argument));
                }
            }

            return instance;
        }
    }
}
