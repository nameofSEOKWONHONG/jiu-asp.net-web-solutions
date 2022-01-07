using Chloe.Reflection;
using System.Data;

namespace Chloe.Infrastructure
{
    public static class MappingTypeSystem
    {
        static readonly object _lockObj = new object();
        static readonly Dictionary<Type, MappingType> _mappingTypes;

        static MappingTypeSystem()
        {
            Dictionary<Type, MappingType> defaultMappingTypes = new Dictionary<Type, MappingType>();

            Add(defaultMappingTypes, new MappingType(typeof(byte)) { DbType = DbType.Byte, DbValueConverter = new Byte_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(sbyte)) { DbType = DbType.SByte, DbValueConverter = new SByte_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(short)) { DbType = DbType.Int16, DbValueConverter = new Int16_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(ushort)) { DbType = DbType.UInt16, DbValueConverter = new UInt16_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(int)) { DbType = DbType.Int32, DbValueConverter = new Int32_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(uint)) { DbType = DbType.UInt32, DbValueConverter = new UInt32_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(long)) { DbType = DbType.Int64, DbValueConverter = new Int64_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(ulong)) { DbType = DbType.UInt64, DbValueConverter = new UInt64_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(float)) { DbType = DbType.Single, DbValueConverter = new Single_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(double)) { DbType = DbType.Double, DbValueConverter = new Double_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(decimal)) { DbType = DbType.Decimal, DbValueConverter = new Decimal_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(bool)) { DbType = DbType.Boolean, DbValueConverter = new Boolean_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(string)) { DbType = DbType.String, DbValueConverter = new String_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(Guid)) { DbType = DbType.Guid, DbValueConverter = new Guid_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(DateTime)) { DbType = DbType.DateTime, DbValueConverter = new DateTime_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(DateTimeOffset)) { DbType = DbType.DateTimeOffset, DbValueConverter = new DateTimeOffset_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(TimeSpan)) { DbType = DbType.Time, DbValueConverter = new TimeSpan_ValueConverter() });
            Add(defaultMappingTypes, new MappingType(typeof(byte[])) { DbType = DbType.Binary, DbValueConverter = new Binary_ValueConverter() });

            _mappingTypes = PublicHelper.Clone(defaultMappingTypes);
        }
        static void Add(Dictionary<Type, MappingType> dic, MappingType mappingType)
        {
            dic.Add(mappingType.Type, mappingType);
        }

        /// <summary>
        /// 配置映射的类型
        /// </summary>
        /// <typeparam name="T">元数据类型</typeparam>
        /// <returns></returns>
        public static MappingTypeBuilder Configure<T>()
        {
            Type type = typeof(T);
            MappingType mappingType;
            if (!_mappingTypes.TryGetValue(type, out mappingType))
            {
                lock (_lockObj)
                {
                    if (!_mappingTypes.TryGetValue(type, out mappingType))
                    {
                        mappingType = new MappingType(type);
                        _mappingTypes[type] = mappingType;
                    }
                }
            }

            return new MappingTypeBuilder(mappingType);
        }

        public static DbType? GetDbType(Type type)
        {
            if (type == null)
                return null;

            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                underlyingType = Enum.GetUnderlyingType(underlyingType);

            MappingType mappingType;
            if (_mappingTypes.TryGetValue(underlyingType, out mappingType))
                return mappingType.DbType;

            return null;
        }
        public static MappingType GetMappingType(Type type)
        {
            MappingType mappingType;
            if (!IsMappingType(type, out mappingType))
            {
                throw new ArgumentException($"The type '{type.FullName}' is not mapping type.");
            }

            return mappingType;
        }
        public static bool IsMappingType(Type type)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                return true;

            return _mappingTypes.ContainsKey(underlyingType);
        }
        public static bool IsMappingType(Type type, out MappingType mappingType)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                underlyingType = Enum.GetUnderlyingType(underlyingType);

            return _mappingTypes.TryGetValue(underlyingType, out mappingType);
        }
    }
}
