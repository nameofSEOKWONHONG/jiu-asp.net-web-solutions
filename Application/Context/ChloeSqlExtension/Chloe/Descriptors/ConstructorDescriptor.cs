using Chloe.Mapper;
using Chloe.Reflection;
using System.Reflection;

namespace Chloe.Descriptors
{
    public class ConstructorDescriptor
    {
        ObjectMemberMapper _mapper = null;
        ObjectConstructor _objectConstructor = null;
        ConstructorDescriptor(ConstructorInfo constructorInfo)
        {
            this.ConstructorInfo = constructorInfo;
            this.Init();
        }

        void Init()
        {
            ConstructorInfo constructor = this.ConstructorInfo;
            Type type = constructor.DeclaringType;

            if (ReflectionExtension.IsAnonymousType(type))
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                this.MemberParameterMap = new Dictionary<MemberInfo, ParameterInfo>(parameters.Length);
                foreach (ParameterInfo parameter in parameters)
                {
                    PropertyInfo prop = type.GetProperty(parameter.Name);
                    this.MemberParameterMap.Add(prop, parameter);
                }
            }
            else
                this.MemberParameterMap = new Dictionary<MemberInfo, ParameterInfo>(0);
        }

        public ConstructorInfo ConstructorInfo { get; private set; }
        public Dictionary<MemberInfo, ParameterInfo> MemberParameterMap { get; private set; }
        public InstanceCreator GetInstanceCreator()
        {
            ObjectConstructor objectConstructor = null;
            if (null == this._objectConstructor)
            {
                this._objectConstructor = ObjectConstructor.GetInstance(this.ConstructorInfo);
            }

            objectConstructor = this._objectConstructor;
            return objectConstructor.InstanceCreator;
        }
        public ObjectMemberMapper GetEntityMemberMapper()
        {
            ObjectMemberMapper mapper = null;
            if (null == this._mapper)
            {
                this._mapper = ObjectMemberMapper.GetInstance(this.ConstructorInfo.DeclaringType);
            }

            mapper = this._mapper;
            return mapper;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<ConstructorInfo, ConstructorDescriptor> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<ConstructorInfo, ConstructorDescriptor>();

        public static ConstructorDescriptor GetInstance(ConstructorInfo constructorInfo)
        {
            ConstructorDescriptor instance;
            if (!InstanceCache.TryGetValue(constructorInfo, out instance))
            {
                lock (constructorInfo)
                {
                    if (!InstanceCache.TryGetValue(constructorInfo, out instance))
                    {
                        instance = new ConstructorDescriptor(constructorInfo);
                        InstanceCache.GetOrAdd(constructorInfo, instance);
                    }
                }
            }

            return instance;
        }
    }

}
