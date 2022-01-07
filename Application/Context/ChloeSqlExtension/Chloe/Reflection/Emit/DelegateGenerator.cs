using Chloe.Data;
using Chloe.Extensions;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Reflection.Emit
{
    public static class DelegateGenerator
    {
        static readonly MethodInfo GetArrayItemMethod = typeof(object[]).GetMethod("GetValue", new Type[] { typeof(int) });

        public static Func<IDataReader, int, object> CreateDataReaderGetValueHandler(Type valueType)
        {
            var reader = Expression.Parameter(typeof(IDataReader), "reader");
            var ordinal = Expression.Parameter(typeof(int), "ordinal");

            var readerMethod = DataReaderConstant.GetReaderMethod(valueType);

            var getValue = Expression.Call(null, readerMethod, reader, ordinal);
            var toObject = Expression.Convert(getValue, typeof(object));

            var lambda = Expression.Lambda<Func<IDataReader, int, object>>(toObject, reader, ordinal);
            var del = lambda.Compile();

            return del;
        }

        public static MemberMapper CreateMapper(MemberInfo member)
        {
            var p = Expression.Parameter(typeof(object), "instance");
            var instance = Expression.Convert(p, member.DeclaringType);
            var reader = Expression.Parameter(typeof(IDataReader), "reader");
            var ordinal = Expression.Parameter(typeof(int), "ordinal");

            var readerMethod = DataReaderConstant.GetReaderMethod(member.GetMemberType());
            var getValue = Expression.Call(null, readerMethod, reader, ordinal);
            var assign = ExpressionExtension.Assign(member, instance, getValue);
            var lambda = Expression.Lambda<MemberMapper>(assign, p, reader, ordinal);

            MemberMapper del = lambda.Compile();

            return del;
        }

        static BinaryExpression MakeAssign(MemberInfo propertyOrField, Expression instance, Expression value)
        {
            var member = MakeMemberExpression(propertyOrField, instance);
            var setValue = Expression.Assign(member, value);
            return setValue;
        }
        static MemberExpression MakeMemberExpression(MemberInfo propertyOrField, Expression instance)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
            {
                var prop = Expression.Property(instance, (PropertyInfo)propertyOrField);
                return prop;
            }

            if (propertyOrField.MemberType == MemberTypes.Field)
            {
                var field = Expression.Field(instance, (FieldInfo)propertyOrField);
                return field;
            }

            throw new ArgumentException();
        }

        public static InstanceCreator CreateCreator(ConstructorInfo constructor)
        {
            var pExp_arguments = Expression.Parameter(typeof(object[]), "arguments");

            ParameterInfo[] parameters = constructor.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                //object obj = arguments[i];
                var obj = Expression.Call(pExp_arguments, GetArrayItemMethod, Expression.Constant(i));
                //T argument = (T)obj;
                var argument = Expression.Convert(obj, parameter.ParameterType);
                arguments.Add(argument);
            }

            var body = Expression.New(constructor, arguments);
            InstanceCreator ret = Expression.Lambda<InstanceCreator>(body, pExp_arguments).Compile();

            return ret;
        }
        public static InstanceCreator CreateActivator(Type type)
        {
            return CreateCreator(type.GetDefaultConstructor());
        }

        public static MemberSetter CreateSetter(MemberInfo propertyOrField)
        {
            ParameterExpression p = Expression.Parameter(typeof(object), "instance");
            ParameterExpression pValue = Expression.Parameter(typeof(object), "value");
            Expression instance = null;
            if (!propertyOrField.IsStaticMember())
            {
                instance = Expression.Convert(p, propertyOrField.DeclaringType);
            }

            var value = Expression.Convert(pValue, propertyOrField.GetMemberType());
            var setValue = MakeAssign(propertyOrField, instance, value);

            Expression body = setValue;

            var lambda = Expression.Lambda<MemberSetter>(body, p, pValue);
            MemberSetter ret = lambda.Compile();

            return ret;
        }
        public static MemberGetter CreateGetter(MemberInfo propertyOrField)
        {
            ParameterExpression p = Expression.Parameter(typeof(object), "a");
            Expression instance = null;
            if (!propertyOrField.IsStaticMember())
            {
                instance = Expression.Convert(p, propertyOrField.DeclaringType);
            }

            var memberAccess = Expression.MakeMemberAccess(instance, propertyOrField);

            Type type = ReflectionExtension.GetMemberType(propertyOrField);

            Expression body = memberAccess;
            if (type.IsValueType)
            {
                body = Expression.Convert(memberAccess, typeof(object));
            }

            var lambda = Expression.Lambda<MemberGetter>(body, p);
            MemberGetter ret = lambda.Compile();

            return ret;
        }

        public static MethodInvoker CreateInvoker(MethodInfo method)
        {
            List<ParameterExpression> parameterExps = new List<ParameterExpression>();
            ParameterExpression p = Expression.Parameter(typeof(object), "instance");
            parameterExps.Add(p);

            ParameterExpression pParameterArray = Expression.Parameter(typeof(object[]), "parameters");
            parameterExps.Add(pParameterArray);

            Expression instance = null;
            if (!method.IsStatic)
            {
                instance = Expression.Convert(p, method.ReflectedType);
            }

            ParameterInfo[] parameters = method.GetParameters();
            List<Expression> argumentExps = new List<Expression>(parameters.Length);

            var getItemMethod = typeof(object[]).GetMethod("GetValue", new Type[] { typeof(int) });

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                //object parameter = parameters[i];
                var parameterExp = Expression.Call(pParameterArray, getItemMethod, Expression.Constant(i));
                //T argument = (T)parameter;
                var argumentExp = Expression.Convert(parameterExp, parameter.ParameterType);
                argumentExps.Add(argumentExp);
            }

            //instance.Method(parameters)
            MethodCallExpression methodCallExp = Expression.Call(instance, method, argumentExps);

            MethodInvoker ret;
            if (method.ReturnType == typeof(void))
            {
                var act = Expression.Lambda<Action<object, object[]>>(methodCallExp, parameterExps).Compile();
                ret = MakeMethodInvoker(act);
            }
            else
            {
                ret = Expression.Lambda<MethodInvoker>(Expression.Convert(methodCallExp, typeof(object)), parameterExps).Compile();
                ret = MakeMethodInvoker(ret);
            }

            return ret;
        }
        static MethodInvoker MakeMethodInvoker(Action<object, object[]> act)
        {
            MethodInvoker ret = (object instance, object[] parameters) =>
            {
                act(instance, parameters ?? ReflectionExtension.EmptyArray);
                return null;
            };

            return ret;
        }
        static MethodInvoker MakeMethodInvoker(MethodInvoker methodInvoker)
        {
            MethodInvoker ret = (object instance, object[] parameters) =>
            {
                object val = methodInvoker(instance, parameters ?? ReflectionExtension.EmptyArray);
                return val;
            };

            return ret;
        }
    }
}
