using System.Reflection;

namespace Chloe.Reflection
{
    public static class ReflectionExtension
    {
        public static readonly object[] EmptyArray = new object[0];

        public static Type GetMemberType(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).PropertyType;

            if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).FieldType;

            throw new ArgumentException();
        }

        public static bool HasPublicSetter(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).GetSetMethod() != null;

            if (propertyOrField.MemberType == MemberTypes.Field && (propertyOrField as FieldInfo).IsPublic)
                return true;

            return false;
        }
        public static bool HasPublicGetter(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).GetGetMethod() != null;

            if (propertyOrField.MemberType == MemberTypes.Field && (propertyOrField as FieldInfo).IsPublic)
                return true;

            return false;
        }

        public static bool IsStaticMember(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
            {
                MethodInfo getter = ((PropertyInfo)propertyOrField).GetMethod;
                return getter.IsStatic;
            }

            if (propertyOrField.MemberType == MemberTypes.Field && (propertyOrField as FieldInfo).IsStatic)
                return true;

            return false;
        }

        public static void SetMemberValue(this MemberInfo propertyOrField, object obj, object value)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                ((PropertyInfo)propertyOrField).SetValue(obj, value, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                ((FieldInfo)propertyOrField).SetValue(obj, value);
            else
                throw new ArgumentException();
        }
        public static object GetMemberValue(this MemberInfo propertyOrField, object obj)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).GetValue(obj, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).GetValue(obj);

            throw new ArgumentException();
        }

        public static void FastSetMemberValue(this MemberInfo propertyOrField, object obj, object value)
        {
            MemberSetter setter = MemberSetterContainer.Get(propertyOrField);
            setter(obj, value);
            return;
        }
        public static object FastGetMemberValue(this MemberInfo propertyOrField, object obj)
        {
            MemberGetter getter = MemberGetterContainer.Get(propertyOrField);
            return getter(obj);
        }

        public static object Invoke(this MethodInfo method, object obj, params object[] parameters)
        {
            return method.Invoke(obj, parameters ?? EmptyArray);
        }
        public static object FastInvoke(this MethodInfo method, object obj, params object[] parameters)
        {
            MethodInvoker invoker = MethodInvokerContainer.Get(method);
            return invoker(obj, parameters);
        }

        public static MemberInfo AsReflectedMemberOf(this MemberInfo propertyOrField, Type type)
        {
            if (propertyOrField.ReflectedType != type)
            {
                MemberInfo tempMember = null;
                if (propertyOrField.MemberType == MemberTypes.Property)
                {
                    tempMember = type.GetProperty(propertyOrField.Name);
                }
                else if (propertyOrField.MemberType == MemberTypes.Field)
                {
                    tempMember = type.GetField(propertyOrField.Name);
                }

                if (tempMember != null)
                    propertyOrField = tempMember;
            }

            return propertyOrField;
        }
        public static bool CanNull(this Type type)
        {
            if (type.IsNullable())
                return true;

            if (type.IsValueType)
                return false;

            return true;
        }
        public static bool IsNullable(this Type type)
        {
            Type underlyingType;
            return IsNullable(type, out underlyingType);
        }
        public static bool IsNullable(this Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }
        public static Type GetUnderlyingType(this Type type)
        {
            Type underlyingType;
            if (!IsNullable(type, out underlyingType))
                underlyingType = type;

            return underlyingType;
        }
        public static bool IsAnonymousType(this Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }

        public static bool IsGenericCollection(this Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type implementedInterface = type.GetInterface("ICollection`1");
            if (implementedInterface == null)
                return false;

            return true;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

#if net40
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo member)
        {
            return member.GetCustomAttributes<Attribute>();
        }
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member)
        {
            return member.GetCustomAttributes<TAttribute>().FirstOrDefault();
        }
        public static bool IsDefined(this MemberInfo member, Type attributeType)
        {
            return member.IsDefined(attributeType, false);
        }
#endif
    }
}
