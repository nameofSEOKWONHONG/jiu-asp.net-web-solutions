using System.Reflection;
using Chloe.Reflection.Emit;

namespace Chloe.Reflection
{
    internal abstract class DelegateFactory
    {
        public abstract InstanceCreator CreateCreator(ConstructorInfo constructor);
        public abstract MemberGetter CreateGetter(MemberInfo propertyOrField);
        public abstract MemberSetter CreateSetter(MemberInfo propertyOrField);
        public abstract MethodInvoker CreateInvoker(MethodInfo method);
        public abstract MemberMapper CreateMapper(MemberInfo propertyOrField);
    }

    internal class DefaultDelegateFactory : DelegateFactory
    {
        static readonly DefaultDelegateFactory _instance = new DefaultDelegateFactory();
        public static DefaultDelegateFactory Instance { get { return _instance; } }


        /* 如果emit创建失败，则表示运行平台有可能不支持 emit */
        static bool AllowEmit { get; set; }

        DefaultDelegateFactory()
        {

        }
        static DefaultDelegateFactory()
        {
            AllowEmit = false;
            try
            {
                EmitDelegateFactory.Instance.CreateGetter(typeof(string).GetProperty("Length"));
                AllowEmit = true;
            }
            catch
            {
            }
        }

        public override InstanceCreator CreateCreator(ConstructorInfo constructor)
        {
            if (AllowEmit)
                return EmitDelegateFactory.Instance.CreateCreator(constructor);

            return ReflectionDelegateFactory.Instance.CreateCreator(constructor);
        }

        public override MemberGetter CreateGetter(MemberInfo propertyOrField)
        {
            if (AllowEmit)
                return EmitDelegateFactory.Instance.CreateGetter(propertyOrField);

            return ReflectionDelegateFactory.Instance.CreateGetter(propertyOrField);
        }

        public override MemberSetter CreateSetter(MemberInfo propertyOrField)
        {
            if (AllowEmit)
                return EmitDelegateFactory.Instance.CreateSetter(propertyOrField);

            return ReflectionDelegateFactory.Instance.CreateSetter(propertyOrField);
        }

        public override MethodInvoker CreateInvoker(MethodInfo method)
        {
            if (AllowEmit)
                return EmitDelegateFactory.Instance.CreateInvoker(method);

            return ReflectionDelegateFactory.Instance.CreateInvoker(method);
        }

        public override MemberMapper CreateMapper(MemberInfo propertyOrField)
        {
            if (AllowEmit)
                return EmitDelegateFactory.Instance.CreateMapper(propertyOrField);

            return ReflectionDelegateFactory.Instance.CreateMapper(propertyOrField);
        }
    }

    internal class EmitDelegateFactory : DelegateFactory
    {
        static readonly EmitDelegateFactory _instance = new EmitDelegateFactory();
        public static EmitDelegateFactory Instance { get { return _instance; } }

        EmitDelegateFactory()
        {

        }

        public override InstanceCreator CreateCreator(ConstructorInfo constructor)
        {
            InstanceCreator creator = DelegateGenerator.CreateCreator(constructor);
            return creator;
        }

        public override MemberGetter CreateGetter(MemberInfo propertyOrField)
        {
            MemberGetter getter = DelegateGenerator.CreateGetter(propertyOrField);
            return getter;
        }

        public override MemberSetter CreateSetter(MemberInfo propertyOrField)
        {
            MemberSetter setter = DelegateGenerator.CreateSetter(propertyOrField);
            return setter;
        }

        public override MethodInvoker CreateInvoker(MethodInfo method)
        {
            MethodInvoker invoker = DelegateGenerator.CreateInvoker(method);
            return invoker;
        }

        public override MemberMapper CreateMapper(MemberInfo propertyOrField)
        {
            MemberMapper mapper = DelegateGenerator.CreateMapper(propertyOrField);
            return mapper;
        }
    }

    internal class ReflectionDelegateFactory : DelegateFactory
    {
        static readonly ReflectionDelegateFactory _instance = new ReflectionDelegateFactory();
        public static ReflectionDelegateFactory Instance { get { return _instance; } }

        ReflectionDelegateFactory()
        {

        }

        public override InstanceCreator CreateCreator(ConstructorInfo constructor)
        {
            InstanceCreator creator = arguments =>
            {
                return constructor.Invoke(arguments);
            };

            return creator;
        }

        public override MemberGetter CreateGetter(MemberInfo propertyOrField)
        {
            MemberGetter getter = instance =>
            {
                return propertyOrField.GetMemberValue(instance);
            };

            return getter;
        }

        public override MemberSetter CreateSetter(MemberInfo propertyOrField)
        {
            MemberSetter setter = (object instance, object value) =>
            {
                propertyOrField.SetMemberValue(instance, value);
            };

            return setter;
        }

        public override MethodInvoker CreateInvoker(MethodInfo method)
        {
            MethodInvoker invoker = (object instance, object[] parameters) =>
            {
                return method.Invoke(instance, parameters);
            };

            return invoker;
        }

        public override MemberMapper CreateMapper(MemberInfo propertyOrField)
        {
            var readerMethod = Data.DataReaderConstant.GetReaderMethod(propertyOrField.GetMemberType());
            MemberMapper mapper = (object instance, System.Data.IDataReader dataReader, int ordinal) =>
            {
                var value = readerMethod.Invoke(null, dataReader, ordinal);
                propertyOrField.SetMemberValue(instance, value);
            };

            return mapper;
        }
    }
}
