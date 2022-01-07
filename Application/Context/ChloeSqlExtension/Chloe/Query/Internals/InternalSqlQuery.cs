using Chloe.Annotations;
using Chloe.Descriptors;
using Chloe.Infrastructure;
using Chloe.Mapper;
using Chloe.Mapper.Activators;
using Chloe.Mapper.Binders;
using Chloe.Query.Mapping;
using Chloe.Reflection;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Chloe.Query.Internals
{
    class InternalSqlQuery<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        DbContext _dbContext;
        string _sql;
        CommandType _cmdType;
        DbParam[] _parameters;

        public InternalSqlQuery(DbContext dbContext, string sql, CommandType cmdType, DbParam[] parameters)
        {
            this._dbContext = dbContext;
            this._sql = sql;
            this._cmdType = cmdType;
            this._parameters = parameters;
        }

        public IEnumerable<T> AsIEnumerable()
        {
            return this;
        }
        public IAsyncEnumerable<T> AsIAsyncEnumerable()
        {
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new QueryEnumerator<T>(this.ExecuteReader, this.CreateObjectActivator);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            IAsyncEnumerator<T> enumerator = this.GetEnumerator() as IAsyncEnumerator<T>;
            return enumerator;
        }

        IObjectActivator CreateObjectActivator(IDataReader dataReader)
        {
            Type type = typeof(T);

            if (type != PublicConstants.TypeOfObject && MappingTypeSystem.IsMappingType(type))
            {
                PrimitiveObjectActivatorCreator activatorCreator = new PrimitiveObjectActivatorCreator(type, 0);
                return activatorCreator.CreateObjectActivator();
            }

            return GetObjectActivator(type, dataReader);
        }

        async Task<IDataReader> ExecuteReader(bool @async)
        {
            IDataReader reader = await this._dbContext.AdoSession.ExecuteReader(this._sql, this._parameters, this._cmdType, @async);
            return reader;
        }

        static IObjectActivator GetObjectActivator(Type type, IDataReader reader)
        {
            if (type == PublicConstants.TypeOfObject || type == typeof(DapperRow))
            {
                return new DapperRowObjectActivator();
            }

            List<CacheInfo> caches;
            if (!ObjectActivatorCache.TryGetValue(type, out caches))
            {
                if (!Monitor.TryEnter(type))
                {
                    return CreateObjectActivator(type, reader);
                }

                try
                {
                    caches = ObjectActivatorCache.GetOrAdd(type, new List<CacheInfo>(1));
                }
                finally
                {
                    Monitor.Exit(type);
                }
            }

            CacheInfo cache = TryGetCacheInfoFromList(caches, reader);

            if (cache == null)
            {
                lock (caches)
                {
                    cache = TryGetCacheInfoFromList(caches, reader);
                    if (cache == null)
                    {
                        ComplexObjectActivator activator = CreateObjectActivator(type, reader);
                        cache = new CacheInfo(activator, reader);
                        caches.Add(cache);
                    }
                }
            }

            return cache.ObjectActivator;
        }
        static ComplexObjectActivator CreateObjectActivator(Type type, IDataReader reader)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException(string.Format("The type of '{0}' does't define a none parameter constructor.", type.FullName));

            ConstructorDescriptor constructorDescriptor = ConstructorDescriptor.GetInstance(constructor);
            ObjectMemberMapper mapper = constructorDescriptor.GetEntityMemberMapper();
            InstanceCreator instanceCreator = constructorDescriptor.GetInstanceCreator();
            List<IMemberBinder> memberBinders = PrepareMemberBinders(type, reader, mapper);

            ComplexObjectActivator objectActivator = new ComplexObjectActivator(instanceCreator, new List<IObjectActivator>(), memberBinders, null);
            objectActivator.Prepare(reader);

            return objectActivator;
        }
        static List<IMemberBinder> PrepareMemberBinders(Type type, IDataReader reader, ObjectMemberMapper mapper)
        {
            List<IMemberBinder> memberBinders = new List<IMemberBinder>(reader.FieldCount);

            MemberInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            MemberInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
            List<MemberInfo> members = new List<MemberInfo>(properties.Length + fields.Length);
            members.AddRange(properties);
            members.AddRange(fields);

            TypeDescriptor typeDescriptor = EntityTypeContainer.TryGetDescriptor(type);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                MemberInfo mapMember = TryGetMapMember(members, name, typeDescriptor);

                if (mapMember == null)
                    continue;

                MRMTuple mMapperTuple = mapper.GetMappingMemberMapper(mapMember);
                if (mMapperTuple == null)
                    continue;

                PrimitiveMemberBinder memberBinder = new PrimitiveMemberBinder(mapMember, mMapperTuple, i);
                memberBinders.Add(memberBinder);
            }

            return memberBinders;
        }

        static MemberInfo TryGetMapMember(List<MemberInfo> members, string readerName, TypeDescriptor typeDescriptor)
        {
            MemberInfo mapMember = null;

            foreach (MemberInfo member in members)
            {
                string columnName = null;
                if (typeDescriptor != null)
                {
                    PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.FindPrimitivePropertyDescriptor(member);
                    if (propertyDescriptor != null)
                        columnName = propertyDescriptor.Column.Name;
                }

                if (string.IsNullOrEmpty(columnName))
                {
                    ColumnAttribute columnAttribute = member.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null)
                        columnName = columnAttribute.Name;
                }

                if (string.IsNullOrEmpty(columnName))
                    continue;

                if (!string.Equals(columnName, readerName, StringComparison.OrdinalIgnoreCase))
                    continue;

                mapMember = member;
                break;
            }

            if (mapMember == null)
            {
                mapMember = members.Find(a => a.Name == readerName);
            }

            if (mapMember == null)
            {
                mapMember = members.Find(a => string.Equals(a.Name, readerName, StringComparison.OrdinalIgnoreCase));
            }

            return mapMember;
        }

        static CacheInfo TryGetCacheInfoFromList(List<CacheInfo> caches, IDataReader reader)
        {
            CacheInfo cache = null;
            for (int i = 0; i < caches.Count; i++)
            {
                var item = caches[i];
                if (item.IsTheSameFields(reader))
                {
                    cache = item;
                    break;
                }
            }

            return cache;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, List<CacheInfo>> ObjectActivatorCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, List<CacheInfo>>();

        public class CacheInfo
        {
            ReaderFieldInfo[] _readerFields;
            ComplexObjectActivator _objectActivator;
            public CacheInfo(ComplexObjectActivator activator, IDataReader reader)
            {
                int fieldCount = reader.FieldCount;
                var readerFields = new ReaderFieldInfo[fieldCount];

                for (int i = 0; i < fieldCount; i++)
                {
                    readerFields[i] = new ReaderFieldInfo(reader.GetName(i), reader.GetFieldType(i));
                }

                this._readerFields = readerFields;
                this._objectActivator = activator;
            }

            public ComplexObjectActivator ObjectActivator { get { return this._objectActivator; } }

            public bool IsTheSameFields(IDataReader reader)
            {
                ReaderFieldInfo[] readerFields = this._readerFields;
                int fieldCount = reader.FieldCount;

                if (fieldCount != readerFields.Length)
                    return false;

                for (int i = 0; i < fieldCount; i++)
                {
                    ReaderFieldInfo readerField = readerFields[i];
                    if (reader.GetFieldType(i) != readerField.Type || reader.GetName(i) != readerField.Name)
                    {
                        return false;
                    }
                }

                return true;
            }

            class ReaderFieldInfo
            {
                string _name;
                Type _type;
                public ReaderFieldInfo(string name, Type type)
                {
                    this._name = name;
                    this._type = type;
                }

                public string Name { get { return this._name; } }
                public Type Type { get { return this._type; } }
            }
        }
    }
}
