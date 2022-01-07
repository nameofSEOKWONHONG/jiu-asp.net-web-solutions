using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.Mapper;
using Chloe.Mapper.Activators;
using Chloe.Mapper.Binders;
using Chloe.Reflection;
using System.Reflection;

namespace Chloe.Query.Mapping
{
    public class ComplexObjectActivatorCreator : IObjectActivatorCreator
    {
        public ComplexObjectActivatorCreator(ConstructorDescriptor constructorDescriptor)
        {
            this.ConstructorDescriptor = constructorDescriptor;
            this.ConstructorParameters = new Dictionary<ParameterInfo, int>();
            this.ConstructorComplexParameters = new Dictionary<ParameterInfo, IObjectActivatorCreator>();
            this.PrimitiveMembers = new Dictionary<MemberInfo, int>();
            this.ComplexMembers = new Dictionary<MemberInfo, IObjectActivatorCreator>();
            this.CollectionMembers = new Dictionary<MemberInfo, IObjectActivatorCreator>();
        }

        public Type ObjectType { get { return this.ConstructorDescriptor.ConstructorInfo.DeclaringType; } }
        public bool IsRoot { get; set; }
        public int? CheckNullOrdinal { get; set; }
        public ConstructorDescriptor ConstructorDescriptor { get; private set; }
        public Dictionary<ParameterInfo, int> ConstructorParameters { get; private set; }
        public Dictionary<ParameterInfo, IObjectActivatorCreator> ConstructorComplexParameters { get; private set; }

        /// <summary>
        /// 映射成员集合。以 MemberInfo 为 key，读取 DataReader 时的 Ordinal 为 value
        /// </summary>
        public Dictionary<MemberInfo, int> PrimitiveMembers { get; private set; }
        /// <summary>
        /// 复杂类型成员集合。
        /// </summary>
        public Dictionary<MemberInfo, IObjectActivatorCreator> ComplexMembers { get; private set; }
        public Dictionary<MemberInfo, IObjectActivatorCreator> CollectionMembers { get; private set; }


        public IObjectActivator CreateObjectActivator()
        {
            return this.CreateObjectActivator(null);
        }
        public IObjectActivator CreateObjectActivator(IDbContext dbContext)
        {
            InstanceCreator instanceCreator = this.ConstructorDescriptor.GetInstanceCreator();

            List<IObjectActivator> argumentActivators = this.CreateArgumentActivators(dbContext);
            List<IMemberBinder> memberBinders = this.CreateMemberBinders(dbContext);

            IObjectActivator objectActivator;
            if (dbContext != null)
                objectActivator = new ObjectActivatorWithTracking(instanceCreator, argumentActivators, memberBinders, this.CheckNullOrdinal, dbContext);
            else
                objectActivator = new ComplexObjectActivator(instanceCreator, argumentActivators, memberBinders, this.CheckNullOrdinal);

            if (this.IsRoot && this.HasMany())
            {
                TypeDescriptor entityTypeDescriptor = EntityTypeContainer.GetDescriptor(this.ObjectType);
                List<Tuple<PropertyDescriptor, int>> keys = new List<Tuple<PropertyDescriptor, int>>(entityTypeDescriptor.PrimaryKeys.Count);
                foreach (PrimitivePropertyDescriptor primaryKey in entityTypeDescriptor.PrimaryKeys)
                {
                    keys.Add(new Tuple<PropertyDescriptor, int>(primaryKey, this.PrimitiveMembers[primaryKey.Definition.Property]));
                }

                IEntityKey entityKey = new EntityKey(keys);
                objectActivator = new RootEntityActivator(objectActivator, this.CreateFitter(dbContext), entityKey);
            }

            return objectActivator;
        }

        List<IMemberBinder> CreateMemberBinders(IDbContext dbContext)
        {
            ObjectMemberMapper mapper = this.ConstructorDescriptor.GetEntityMemberMapper();
            List<IMemberBinder> memberBinders = new List<IMemberBinder>(this.PrimitiveMembers.Count + this.ComplexMembers.Count + this.CollectionMembers.Count);
            foreach (var kv in this.PrimitiveMembers)
            {
                MRMTuple mrmTuple = mapper.GetMappingMemberMapper(kv.Key);
                PrimitiveMemberBinder binder = new PrimitiveMemberBinder(kv.Key, mrmTuple, kv.Value);
                memberBinders.Add(binder);
            }

            foreach (var kv in this.ComplexMembers)
            {
                MemberSetter setter = mapper.GetMemberSetter(kv.Key);
                IObjectActivator memberActivtor = kv.Value.CreateObjectActivator(dbContext);
                ComplexMemberBinder binder = new ComplexMemberBinder(setter, memberActivtor);
                memberBinders.Add(binder);
            }

            foreach (var kv in this.CollectionMembers)
            {
                MemberSetter setter = mapper.GetMemberSetter(kv.Key);
                IObjectActivator memberActivtor = kv.Value.CreateObjectActivator(dbContext);
                CollectionMemberBinder binder = new CollectionMemberBinder(setter, memberActivtor);
                memberBinders.Add(binder);
            }

            return memberBinders;
        }
        List<IObjectActivator> CreateArgumentActivators(IDbContext dbContext)
        {
            ParameterInfo[] parameters = this.ConstructorDescriptor.ConstructorInfo.GetParameters();
            List<IObjectActivator> argumentActivators = new List<IObjectActivator>(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                IObjectActivator argumentActivator = null;
                ParameterInfo parameter = parameters[i];
                if (this.ConstructorParameters.TryGetValue(parameter, out int ordinal))
                {
                    argumentActivator = new PrimitiveObjectActivator(parameter.ParameterType, ordinal);
                }
                else if (this.ConstructorComplexParameters.TryGetValue(parameter, out IObjectActivatorCreator argumentActivatorCreator))
                {
                    argumentActivator = argumentActivatorCreator.CreateObjectActivator(dbContext);
                }
                else
                {
                    throw new UnbelievableException();
                }

                argumentActivators.Add(argumentActivator);
            }

            return argumentActivators;
        }

        public IFitter CreateFitter(IDbContext dbContext)
        {
            List<Tuple<PropertyDescriptor, IFitter>> includings = new List<Tuple<PropertyDescriptor, IFitter>>();
            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(this.ConstructorDescriptor.ConstructorInfo.DeclaringType);
            foreach (var item in this.ComplexMembers.Concat(this.CollectionMembers))
            {
                IFitter propFitter = item.Value.CreateFitter(dbContext);
                includings.Add(new Tuple<PropertyDescriptor, IFitter>(typeDescriptor.GetPropertyDescriptor(item.Key), propFitter));
            }

            ComplexObjectFitter fitter = new ComplexObjectFitter(includings);
            return fitter;
        }

        public bool HasMany()
        {
            if (this.CollectionMembers.Count > 0)
                return true;

            foreach (var kv in this.ComplexMembers)
            {
                ComplexObjectActivatorCreator activatorCreator = kv.Value as ComplexObjectActivatorCreator;
                if (activatorCreator.HasMany())
                    return true;
            }

            return false;
        }
    }
}
